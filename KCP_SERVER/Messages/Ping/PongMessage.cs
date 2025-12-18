using System;
using KCP_SERVER.Network;

namespace KCP_SERVER.Messages.Ping
{
    public sealed class PongMessage : IMessage
    {
        public byte Opcode => (byte)MessageOpcode.Pong;
        public ulong ClientTime;

        public void Deserialize(ReadOnlySpan<byte> data)
        {
            ClientTime = BitConverter.ToUInt64(data);
        }

        public byte[] Serialize()
        {
            Span<byte> buf = stackalloc byte[1 + 8];
            buf[0] = Opcode;
            BitConverter.TryWriteBytes(buf.Slice(1), ClientTime);
            return buf.ToArray();
        }

        public void Handle(ClientSession session)
        {
            ulong now = (ulong)Environment.TickCount64;
            Console.WriteLine($"[APP RTT] {now - ClientTime} ms");
        }
    }
}
