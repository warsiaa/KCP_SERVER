using System;
using KCP_SERVER.Network;

namespace KCP_SERVER.Messages
{
    public static class MessageDispatcher
    {
        public static void Dispatch(ClientSession session, byte[] payload)
        {
            // 1) minimum kontrol
            if (payload == null || payload.Length < 1)
            {
                Console.WriteLine("[MSG] Empty payload");
                return;
            }

            // 2) opcode oku
            byte opcode = payload[0];

            // 3) body (opcode sonrası)
            ReadOnlySpan<byte> body = payload.AsSpan(1);

            // 4) factory bul
            if (!MessageRegistry.TryCreate(opcode, out var msg))
            {
                Console.WriteLine($"[MSG] Unknown opcode: 0x{opcode:X2}");
                // istersen burada session'ı drop/kick edersin
                return;
            }

            // 5) deserialize
            try
            {
                msg.Deserialize(body);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MSG] Deserialize failed for 0x{opcode:X2}: {ex.Message}");
                // istersen kick
                return;
            }

            // 6) handle
            try
            {
                msg.Handle(session);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MSG] Handle failed for 0x{opcode:X2}: {ex.Message}");
                // istersen kick
            }
        }
    }
}
