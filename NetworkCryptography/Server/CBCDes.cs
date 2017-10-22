using System;
using NetworkCryptography.Core.DataStructures;
using NetworkCryptography.Core.Helpers;

namespace NetworkCryptography.Server
{
    public class CBCDes
    {
        private static readonly Random rng = new Random();

        public static PaddedBuffer Encrypt(string plaintext, ulong key) => EncryptBlocks(new PaddedBuffer(plaintext), key);
        
        private static PaddedBuffer EncryptBlocks(PaddedBuffer plaintext, ulong key)
        {
            BlockBuffer inputBuffer = new BlockBuffer(plaintext.Count + 1);
            inputBuffer.BlockCopy(plaintext.Data, 0, 1, plaintext.Count);
            inputBuffer.Set(0, GenerateRandomBlock());

            BlockBuffer outputBuffer = new BlockBuffer(inputBuffer.Count);
            ulong currentBlock = GenerateRandomBlock();
            for (int i = 0; i < inputBuffer.Count; i++)
            {
                ulong block = inputBuffer[i];
                block ^= currentBlock;

                currentBlock = DES.Encode(block, key);
                outputBuffer[i] = currentBlock;
            }

            plaintext.Data = outputBuffer;
            return plaintext;
        }

        public static string Decrypt(PaddedBuffer ciphertext, ulong key) => DecryptBlocks(ciphertext, key).ToString();

        private static PaddedBuffer DecryptBlocks(PaddedBuffer ciphertext, ulong key)
        {
            BlockBuffer outputBuffer = new BlockBuffer(ciphertext.Count - 1);
            ulong currentBlock = ciphertext[0];

            for (int i = 1; i < ciphertext.Count; i ++)
            {
                ulong block = ciphertext[i];
                ulong decoded = DES.Decode(block, key);
                ulong plaintextBuffer = currentBlock ^ decoded;

                currentBlock = block;
                outputBuffer[i - 1] = plaintextBuffer;
            }

            ciphertext.Data = outputBuffer;
            return ciphertext;
        }

        private static ulong GenerateRandomBlock()
        {
            byte[] buffer = new byte[64 / 8];
            rng.NextBytes(buffer);

            return BitConverter.ToUInt64(buffer, 0);
        }
    }
}
