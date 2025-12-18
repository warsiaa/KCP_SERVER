namespace KCP_SERVER.Messages
{
    public enum MessageOpcode : byte
    {
        Ping = 0x02,
        Pong = 0x03,
        VersionCheckRequest = 0x10,
        VersionCheckResponse = 0x11,
        Move = 0x10
    }
}
