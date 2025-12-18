using System.Text;
using KCP_SERVER.Network;

namespace KCP_SERVER.Protocol
{
    public static class MessageHandler
    {
        public static void Handle(ClientSession client, byte[] payload)
        {
            var type = (MessageType)payload[0];

            switch (type)
            {
                case MessageType.Hello:
                    var version = Encoding.UTF8.GetString(payload, 1, payload.Length - 1);
                    client.Send(
                        MessageBuilder.Build(
                            MessageType.HelloAck,
                            Encoding.UTF8.GetBytes(version)
                        )
                    );
                    break;

                case MessageType.Ping:
                    client.Send(
                        MessageBuilder.Build(
                            MessageType.Pong,
                            payload[1..]
                        )
                    );
                    break;
            }
        }
    }
}
