using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using KCP_SERVER.Protocol;
using KCP_SERVER.Utils;

namespace KCP_SERVER.Network
{
    public sealed class KcpServer : IDisposable
    {
        private readonly int _port;
        private readonly UdpClient _udp;
        private readonly CancellationTokenSource _cts = new();

        private readonly ConcurrentDictionary<IPEndPoint, ClientSession> _clients = new();
        private Task? _receiveTask;
        private Task? _updateTask;
        private bool _stopped;
        private readonly List<double> _latencySamples = new();
        private uint _lastMetricsTimestamp;
        private int _packetLossCounter;
        private int _timeoutCounter;

        private const uint MetricsIntervalMs = 1000;
        private const uint ClientTimeoutMs = 10000;

        public event Action<string>? Log;
        public event Action<int>? ClientCountChanged;
        public event Action<MetricSample>? MetricsUpdated;

        public int ClientCount => _clients.Count;

        public KcpServer(int port)
        {
            _port = port;
            _udp = new UdpClient(new IPEndPoint(IPAddress.Any, port));
            _lastMetricsTimestamp = Time.Now;
        }

        public void Start()
        {
            if (_stopped)
                throw new ObjectDisposedException(nameof(KcpServer));

            LogMessage($"[SERVER] Listening on port {_port}");
            _receiveTask = Task.Run(ReceiveLoop);
            _updateTask = Task.Run(UpdateLoop);
        }

        public void Stop()
        {
            if (_cts.IsCancellationRequested)
                return;

            _cts.Cancel();
            _udp.Close();

            WaitForBackgroundTasks();

            foreach (var client in _clients.Values)
            {
                client.Dispose();
            }

            _clients.Clear();
            NotifyClientCountChanged();
            LogMessage("[SERVER] Stopped.");
        }

        private void WaitForBackgroundTasks()
        {
            var tasks = new List<Task>();
            if (_receiveTask != null)
                tasks.Add(_receiveTask);
            if (_updateTask != null)
                tasks.Add(_updateTask);

            if (tasks.Count == 0)
                return;

            try
            {
                Task.WaitAll(tasks.ToArray(), 1000);
            }
            catch (AggregateException) { }
            catch (Exception ex)
            {
                LogMessage($"[ERROR] Waiting tasks: {ex.Message}");
            }
        }

        private async Task ReceiveLoop()
        {
            while (!_cts.IsCancellationRequested)
            {
                try
                {
                    var result = await _udp.ReceiveAsync(_cts.Token);

                    bool isNewClient = false;
                    var session = _clients.GetOrAdd(
                        result.RemoteEndPoint,
                        ep =>
                        {
                            isNewClient = true;
                            return new ClientSession(ep, _udp, LogMessage);
                        }
                    );

                    if (isNewClient)
                    {
                        LogMessage($"[CLIENT] {result.RemoteEndPoint} bağlandı.");
                        NotifyClientCountChanged();
                    }

                    bool accepted = session.Input(result.Buffer);
                    if (!accepted)
                    {
                        Interlocked.Increment(ref _packetLossCounter);
                    }
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception ex)
                {
                    LogMessage($"[ERROR] ReceiveLoop: {ex.Message}");
                }
            }
        }

        private async Task UpdateLoop()
        {
            while (!_cts.IsCancellationRequested)
            {
                try
                {
                    uint now = Time.Now;

                    foreach (var kv in _clients)
                    {
                        kv.Value.Update(now);
                        kv.Value.ProcessMessages(MessageHandler.Handle);

                        var latency = kv.Value.GetLatency();
                        if (latency.HasValue)
                        {
                            _latencySamples.Add(latency.Value);
                        }
                    }

                    RemoveTimedOutClients(now);
                    PublishMetricsIfNeeded(now);

                    await Task.Delay(1, _cts.Token);
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception ex)
                {
                    LogMessage($"[ERROR] UpdateLoop: {ex.Message}");
                }
            }
        }

        private void NotifyClientCountChanged()
        {
            ClientCountChanged?.Invoke(_clients.Count);
        }

        private void RemoveTimedOutClients(uint now)
        {
            var toRemove = new List<IPEndPoint>();

            foreach (var client in _clients)
            {
                if (now - client.Value.LastReceiveAt > ClientTimeoutMs)
                {
                    toRemove.Add(client.Key);
                }
            }

            foreach (var endpoint in toRemove)
            {
                if (_clients.TryRemove(endpoint, out var session))
                {
                    session.Dispose();
                    Interlocked.Increment(ref _timeoutCounter);
                    LogMessage($"[CLIENT] {endpoint} bağlantısı zaman aşımına uğradı.");
                }
            }

            if (toRemove.Count > 0)
            {
                NotifyClientCountChanged();
            }
        }

        private void PublishMetricsIfNeeded(uint now)
        {
            if (now - _lastMetricsTimestamp < MetricsIntervalMs)
                return;

            double averageLatency = 0;
            if (_latencySamples.Count > 0)
            {
                double total = 0;
                for (int i = 0; i < _latencySamples.Count; i++)
                {
                    total += _latencySamples[i];
                }

                averageLatency = total / _latencySamples.Count;
            }

            var sample = new MetricSample(
                DateTime.Now,
                Interlocked.Exchange(ref _packetLossCounter, 0),
                averageLatency,
                Interlocked.Exchange(ref _timeoutCounter, 0)
            );

            _latencySamples.Clear();
            _lastMetricsTimestamp = now;

            MetricsUpdated?.Invoke(sample);
        }

        private void LogMessage(string message)
        {
            Log?.Invoke(message);
        }

        public void Dispose()
        {
            if (_stopped)
                return;

            Stop();
            _udp.Dispose();
            _cts.Dispose();
            _stopped = true;
        }
    }
}
