using System;
using System.Buffers;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using kcp;
using KCP_SERVER.Protocol;
using KCP_SERVER.Utils;

namespace KCP_SERVER.Network
{
    public sealed unsafe class ClientSession : IDisposable
    {
        public IPEndPoint Remote { get; }

        private readonly UdpClient _udp;
        private readonly IKCPCB* _kcp;          // KCP state (POINTER)
        private readonly GCHandle _selfHandle;  // user pointer için
        private readonly List<byte> _rxBuffer = new();
        private readonly Action<string> _log;

        public ClientSession(IPEndPoint remote, UdpClient udp, Action<string> log)
        {
            Remote = remote;
            _udp = udp;
            _log = log;

            _selfHandle = GCHandle.Alloc(this);

            _kcp = KCP.ikcp_create(
                1,
                (void*)GCHandle.ToIntPtr(_selfHandle)
            );

            _kcp->output = &KcpOutput;

            // ================================
            // KCP CONFIG (SADECE DOĞRU ALANLAR)
            // ================================

            // Window size
            _kcp->snd_wnd = 128;
            _kcp->rcv_wnd = 128;

            // MTU
            _kcp->mtu = 1200;

            // Latency / nodelay tuning
            _kcp->nodelay = 1;
            _kcp->interval = 10;
            _kcp->fastresend = 2;
            _kcp->nocwnd = 1;

            // Dead link detection
            _kcp->dead_link = 20;
        }


        // ================================
        // UDP -> KCP
        // ================================
        public void Input(byte[] data)
        {
            fixed (byte* p = data)
            {
                KCP.ikcp_input(_kcp, p, data.Length);
            }
        }

        // ================================
        // TICK (her frame / loop)
        // ================================
        public void Update(uint now)
        {
            KCP.ikcp_update(_kcp, now);
        }

        // ================================
        // KCP -> APPLICATION
        // ================================
        public void ProcessMessages(Action<ClientSession, byte[]> onMessage)
        {
            byte[] temp = ArrayPool<byte>.Shared.Rent(64 * 1024);

            try
            {
                fixed (byte* p = temp)
                {
                    while (true)
                    {
                        int len = KCP.ikcp_recv(_kcp, p, temp.Length);
                        if (len <= 0)
                            break;

                        for (int i = 0; i < len; i++)
                            _rxBuffer.Add(temp[i]);

                        // length-prefix (uint16 LE)
                        while (_rxBuffer.Count >= 2)
                        {
                            int msgLen = _rxBuffer[0] | (_rxBuffer[1] << 8);
                            if (_rxBuffer.Count < msgLen + 2)
                                break;

                            byte[] msg = _rxBuffer.GetRange(2, msgLen).ToArray();
                            _rxBuffer.RemoveRange(0, msgLen + 2);

                            try
                            {
                                var decrypted = PacketCrypto.Decrypt(msg);
                                onMessage(this, decrypted);
                            }
                            catch (CryptographicException)
                            {
                                _log($"[SECURITY] Dropped invalid packet from {Remote}.");
                            }
                        }
                    }
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(temp);
            }
        }

        // ================================
        // APPLICATION -> KCP
        // ================================
        public void Send(byte[] payload)
        {
            if (payload.Length + PacketCrypto.EncryptionOverhead > ushort.MaxValue)
            {
                throw new InvalidOperationException(
                    $"Payload too large to encrypt and frame. Max plaintext length is {ushort.MaxValue - PacketCrypto.EncryptionOverhead} bytes."
                );
            }

            var encrypted = PacketCrypto.Encrypt(payload);
            var framed = MessageBuilder.Frame(encrypted);

            fixed (byte* p = framed)
            {
                KCP.ikcp_send(_kcp, p, framed.Length);
            }
        }

        // ================================
        // KCP OUTPUT CALLBACK
        // ================================
        private static int KcpOutput(
            byte* buf,
            int len,
            IKCPCB* kcp,
            void* user
        )
        {
            var handle = GCHandle.FromIntPtr((IntPtr)user);
            var self = (ClientSession)handle.Target!;

            byte[] managed = new byte[len];
            Marshal.Copy((IntPtr)buf, managed, 0, len);
            self._udp.Send(managed, len, self.Remote);

            return 0;
        }

        // ================================
        // CLEANUP
        // ================================
        public void Dispose()
        {
            KCP.ikcp_release(_kcp);
            _selfHandle.Free();
        }
    }
}
