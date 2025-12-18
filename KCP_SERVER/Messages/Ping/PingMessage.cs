using System;
using KCP_SERVER.Network;

namespace KCP_SERVER.Messages.Ping
{
    public sealed class PingMessage : IMessage
    {
        public byte Opcode => (byte)MessageOpcode.Ping;

        private ulong _clientTime;

        public void Deserialize(ReadOnlySpan<byte> data)
        {
            _clientTime = BitConverter.ToUInt64(data);
        }

        public byte[] Serialize()
        {
            Span<byte> buf = stackalloc byte[1 + 8];
            buf[0] = Opcode;
            BitConverter.TryWriteBytes(buf.Slice(1), _clientTime);
            return buf.ToArray();
        }

        public void Handle(ClientSession session)
        {
            // Pong geri yolla
            var pong = new PongMessage { ClientTime = _clientTime };
            session.Send(pong.Serialize());
        }
    }
}
