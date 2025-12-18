using System;
using KCP_SERVER.Network;

namespace KCP_SERVER.Messages.Versioning
{
    public sealed class VersionCheckRequest : IMessage
    {
        public byte Opcode => (byte)MessageOpcode.VersionCheckRequest;

        public byte Major;
        public byte Minor;
        public byte Patch;

        // ============================
        // Deserialize (client -> server)
        // ============================
        public void Deserialize(ReadOnlySpan<byte> data)
        {
            if (data.Length < 3)
                throw new Exception("VersionCheckRequest: invalid payload");

            Major = data[0];
            Minor = data[1];
            Patch = data[2];
        }

        // ============================
        // Serialize (client tarafında)
        // ============================
        public byte[] Serialize()
        {
            return new byte[]
            {
                Opcode,
                Major,
                Minor,
                Patch
            };
        }

        // ============================
        // Handle (server logic)
        // ============================
        public void Handle(ClientSession session)
        {
            // Server version (tek yerden yönet)
            const byte SERVER_MAJOR = 1;
            const byte SERVER_MINOR = 0;
            const byte SERVER_PATCH = 0;

            bool ok =
                Major == SERVER_MAJOR &&
                Minor == SERVER_MINOR &&
                Patch == SERVER_PATCH;

            var response = new VersionCheckResponse
            {
                Accepted = ok,
                ServerMajor = SERVER_MAJOR,
                ServerMinor = SERVER_MINOR,
                ServerPatch = SERVER_PATCH
            };

            session.Send(response.Serialize());

            if (!ok)
            {
                Console.WriteLine(
                    $"[VERSION] Mismatch. Client={Major}.{Minor}.{Patch} " +
                    $"Server={SERVER_MAJOR}.{SERVER_MINOR}.{SERVER_PATCH}"
                );

                // İstersen burada disconnect edebilirsin
                // session.Disconnect("Version mismatch");
            }
            else
            {
                Console.WriteLine(
                    $"[VERSION] Accepted {Major}.{Minor}.{Patch}"
                );
            }
        }
    }
}
