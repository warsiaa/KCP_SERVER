using System;

namespace KCP_SERVER.Messages.Versioning
{
    public sealed class VersionCheckResponse : IMessage
    {
        public byte Opcode => (byte)MessageOpcode.VersionCheckResponse;

        public bool Accepted;
        public byte ServerMajor;
        public byte ServerMinor;
        public byte ServerPatch;

        // ============================
        // Deserialize (server -> client)
        // ============================
        public void Deserialize(ReadOnlySpan<byte> data)
        {
            if (data.Length < 4)
                throw new Exception("VersionCheckResponse: invalid payload");

            Accepted = data[0] == 1;
            ServerMajor = data[1];
            ServerMinor = data[2];
            ServerPatch = data[3];
        }

        // ============================
        // Serialize
        // ============================
        public byte[] Serialize()
        {
            return new byte[]
            {
                Opcode,
                (byte)(Accepted ? 1 : 0),
                ServerMajor,
                ServerMinor,
                ServerPatch
            };
        }

        // ============================
        // Handle (server tarafında boş)
        // ============================
        public void Handle(Network.ClientSession session)
        {
            // Server bu mesajı handle etmez
        }
    }
}
