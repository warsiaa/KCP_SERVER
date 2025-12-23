using System;
using System.Security.Cryptography;
using System.Text;

namespace KCP_SERVER.Utils
{
    public static class PacketCrypto
    {
        private const int NonceSize = 12;
        private const int TagSize = 16;
        public const int EncryptionOverhead = NonceSize + TagSize;
        private static readonly byte[] Key = BuildKey();

        public static byte[] Encrypt(byte[] plaintext)
        {
            byte[] nonce = new byte[NonceSize];
            RandomNumberGenerator.Fill(nonce);

            byte[] ciphertext = new byte[plaintext.Length];
            byte[] tag = new byte[TagSize];

            using var aes = new AesGcm(Key);
            aes.Encrypt(nonce, plaintext, ciphertext, tag);

            byte[] result = new byte[NonceSize + TagSize + ciphertext.Length];
            Buffer.BlockCopy(nonce, 0, result, 0, NonceSize);
            Buffer.BlockCopy(tag, 0, result, NonceSize, TagSize);
            Buffer.BlockCopy(ciphertext, 0, result, NonceSize + TagSize, ciphertext.Length);
            return result;
        }

        public static byte[] Decrypt(byte[] payload)
        {
            if (payload.Length < NonceSize + TagSize)
            {
                throw new CryptographicException("Payload too small.");
            }

            byte[] nonce = new byte[NonceSize];
            byte[] tag = new byte[TagSize];
            byte[] ciphertext = new byte[payload.Length - NonceSize - TagSize];

            Buffer.BlockCopy(payload, 0, nonce, 0, NonceSize);
            Buffer.BlockCopy(payload, NonceSize, tag, 0, TagSize);
            Buffer.BlockCopy(payload, NonceSize + TagSize, ciphertext, 0, ciphertext.Length);

            byte[] plaintext = new byte[ciphertext.Length];
            using var aes = new AesGcm(Key);
            aes.Decrypt(nonce, ciphertext, tag, plaintext);
            return plaintext;
        }

        private static byte[] BuildKey()
        {
            const string defaultKey = "kcp-server-default-key-change-me";
            string? configuredKey = Environment.GetEnvironmentVariable("KCP_PACKET_KEY");
            string source = string.IsNullOrWhiteSpace(configuredKey) ? defaultKey : configuredKey;
            return SHA256.HashData(Encoding.UTF8.GetBytes(source));
        }
    }
}
