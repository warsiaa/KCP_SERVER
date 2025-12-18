using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using KCP_SERVER.Protocol;
using KCP_SERVER.Utils;

namespace KCP_SERVER.Network
{
    public sealed class KcpServer
    {
        private readonly int _port;
        private readonly UdpClient _udp;
        private readonly CancellationTokenSource _cts = new();

        private readonly ConcurrentDictionary<IPEndPoint, ClientSession> _clients = new();

        public KcpServer(int port)
        {
            _port = port;
            _udp = new UdpClient(new IPEndPoint(IPAddress.Any, port));
        }

        public void Start()
        {
            Console.WriteLine($"[SERVER] Listening on port {_port}");
            Task.Run(ReceiveLoop);
            Task.Run(UpdateLoop);
        }

        public void Stop()
        {
            _cts.Cancel();
            _udp.Close();
        }

        private async Task ReceiveLoop()
        {
            while (!_cts.IsCancellationRequested)
            {
                try
                {
                    var result = await _udp.ReceiveAsync(_cts.Token);

                    var session = _clients.GetOrAdd(
                        result.RemoteEndPoint,
                        ep => new ClientSession(ep, _udp)
                    );

                    session.Input(result.Buffer);
                }
                catch { }
            }
        }

        private async Task UpdateLoop()
        {
            while (!_cts.IsCancellationRequested)
            {
                uint now = Time.Now;

                foreach (var kv in _clients)
                {
                    kv.Value.Update(now);
                    kv.Value.ProcessMessages(MessageHandler.Handle);
                }

                await Task.Delay(1);
            }
        }
    }
}
