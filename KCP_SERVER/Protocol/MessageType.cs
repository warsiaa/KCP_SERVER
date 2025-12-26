namespace KCP_SERVER.Protocol
{
    public enum MessageType : byte
    {
        Hello = 1,
        HelloAck = 2,
        Ping = 3,
        Pong = 4,
        VersionCheckRequest = 5,
        VersionCheckResponse = 6
    }
}
