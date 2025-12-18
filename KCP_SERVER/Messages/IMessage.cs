namespace KCP_SERVER.Messages
{
    public interface IMessage
    {
        byte Opcode { get; }

        void Deserialize(ReadOnlySpan<byte> data);
        byte[] Serialize();
        void Handle(Network.ClientSession session);
    }
}
