using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace KCP_SERVER.Messages
{
    public static class MessageRegistry
    {
        private static readonly Dictionary<byte, Func<IMessage>> _factories = new();

        static MessageRegistry()
        {
            Register(MessageOpcode.Ping, () => new Ping.PingMessage());
            Register(MessageOpcode.Pong, () => new Ping.PongMessage());
            Register(MessageOpcode.VersionCheckRequest, () => new Versioning.VersionCheckRequest());
            Register(MessageOpcode.VersionCheckResponse, () => new Versioning.VersionCheckResponse());
        }

        private static void Register(MessageOpcode opcode, Func<IMessage> factory)
        {
            _factories[(byte)opcode] = factory;
        }

        public static bool TryCreate(byte opcode, out IMessage message)
        {
            if (_factories.TryGetValue(opcode, out var factory))
            {
                message = factory();
                return true;
            }

            message = null!;
            return false;
        }
    }
}
