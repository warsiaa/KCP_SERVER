using KCP_SERVER.Protocol;
using System;

namespace KCP_SERVER.Protocol
{
    public static class MessageBuilder
    {
        public static byte[] Build(MessageType type, byte[] data)
        {
            var msg = new byte[1 + data.Length];
            msg[0] = (byte)type;
            Buffer.BlockCopy(data, 0, msg, 1, data.Length);
            return msg;
        }

        public static byte[] Frame(byte[] payload)
        {
            var buf = new byte[payload.Length + 2];
            buf[0] = (byte)(payload.Length & 0xFF);
            buf[1] = (byte)(payload.Length >> 8);
            Buffer.BlockCopy(payload, 0, buf, 2, payload.Length);
            return buf;
        }
    }
}
