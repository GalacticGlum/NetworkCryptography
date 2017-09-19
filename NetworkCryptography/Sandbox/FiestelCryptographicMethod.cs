/*
 * Author: Shon Verch
 * File Name: FiestelCryptographicMethod.cs
 * Project Name: NetworkCryptography
 * Creation Date: 9/19/2017
 * Modified Date: 9/19/2017
 * Description: Generic Fiestel cipher implementation which allows clients to specify the permutation, key, and round functions.
 */

using System;
using System.Collections;
using System.IO;
using System.Text;
using Sandbox.Helpers;

namespace Sandbox
{
    /// <summary>
    /// Generic Fiestel cipher implementation which allows clients to specify the permutation, key, and round functions.
    /// </summary>
    public abstract class FiestelCryptographicMethod : ICryptographicMethod
    {
        /// <summary>
        /// The keys used for encryption and decryption.
        /// </summary>
        /// 
        public byte[] Keys { get; }

        /// <summary>
        /// The amount of rounds.
        /// </summary>
        public int Rounds { get; }

        /// <summary>
        /// Size of block in bits.
        /// </summary>
        public int BlockSize { get; }

        private readonly Random random;

        protected abstract void InitialPermutation(BitSet cipherText);
        protected abstract void FinalPermutation(BitSet cipherText);
       
        protected abstract BitSet Round(BitSet right, BitSet key);
        protected abstract BitSet GetEncryptionKey(BitSet key, int round);
        protected abstract BitSet GetDecryptionKey(BitSet key, int round);
        
        public FiestelCryptographicMethod(byte[] keys, int rounds, int blockSize)
        {
            BlockSize = blockSize;
            Keys = keys;
            Rounds = rounds;

            random = new Random();
        }

        /// <summary>
        /// Encrypt a message. 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public string Encrypt(string message)
        {
            // Create our block buffer
            int blockLength = BlockSize / (sizeof(byte) * 8);
            byte[] blockBuffer = new byte[blockLength];

            MemoryStream dataBuffer = new MemoryStream(Encoding.UTF8.GetBytes(message));
            int bytesRead = dataBuffer.Read(blockBuffer, 0, blockLength);

            BitSet keyBuffer = new BitSet(Keys);
            BitSet currentCipherBlock = CreateInitializationVector();
            MemoryStream outputBuffer = new MemoryStream(currentCipherBlock.ToBytes(), 0, blockLength, true, true);

            while (bytesRead > 0)
            {
                if (bytesRead < blockLength)
                {
                    Pad(blockBuffer, bytesRead, blockLength);
                }

                BitSet blockBits = new BitSet(blockBuffer);
                blockBits.Xor(currentCipherBlock);
                currentCipherBlock = EncryptBlock(blockBits, keyBuffer);
                outputBuffer.Write(currentCipherBlock.ToBytes(0, blockLength), 0, blockLength);

                bytesRead = dataBuffer.Read(blockBuffer, 0, blockLength);
            }

            return Encoding.UTF8.GetString(outputBuffer.GetBuffer());
        }

        public string Decrypt(string encryptedMessage)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Encrypt a block of data.
        /// </summary>
        /// <param name="block">The block to encrypt.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        private BitSet EncryptBlock(BitSet block, BitSet key)
        {
            // Create a copy of the data before we begin manipulating it.
            BitSet cipherBuffer = (BitSet)block.Clone();
            BitSet keyBuffer = (BitSet) key.Clone();

            InitialPermutation(cipherBuffer);

            BitSet left = cipherBuffer.GetBits(0, BlockSize / 2);
            BitSet right = cipherBuffer.GetBits(BlockSize / 2, BlockSize);

            for (int i = 0; i < Rounds; i++)
            {
                StepRound(left, right, GetEncryptionKey(keyBuffer, i));
            }

            SwapBits(left, right);
            cipherBuffer.CopyFrom(0, left, BlockSize / 2);
            cipherBuffer.CopyFrom(BlockSize / 2, right, BlockSize / 2);
            
            FinalPermutation(cipherBuffer);
            return cipherBuffer;
        }

        /// <summary>
        /// Step a round in the Fiestel cipher.
        /// </summary>
        /// <param name="left">The left segment.</param>
        /// <param name="right">The right segment.</param>
        /// <param name="key">The key.</param>
        private void StepRound(BitSet left, BitSet right, BitSet key)
        {
            left.Xor(Round(right, key));
            SwapBits(left, right);
        }

        /// <summary>
        /// Swap two block segments.
        /// </summary>
        /// <param name="left">The left segment.</param>
        /// <param name="right">The right segment.</param>
        private void SwapBits(BitSet left, BitSet right)
        {
            for (int i = 0; i < BlockSize / 2; i++)
            {
                bool temp = left.Get(i);
                left.Set(i, right.Get(i));
                right.Set(i, temp);
            }
        }

        /// <summary>
        /// Create the initialization vector.
        /// The initialization vector is just a block filled with random values.
        /// </summary>
        /// <returns></returns>
        private BitSet CreateInitializationVector()
        {
            byte[] buffer = new byte[BlockSize];
            random.NextBytes(buffer);

            return new BitSet(buffer);
        }

        /// <summary>
        /// Pad a byte array to a certain length with a random value.
        /// </summary>
        /// <param name="buffer">The byte array to pad.</param>
        /// <param name="dataSize">The size of the data inside the buffer.</param>
        /// <param name="targetSize">The target size of the whole buffer.</param>
        private static void Pad(byte[] buffer, int dataSize, int targetSize)
        {
            // Random value to pad the buffer with; we are just using the delta size of the buffer and target.
            byte padValue = (byte)(targetSize - dataSize);
            for (int i = dataSize; i < targetSize; i++)
            {
                buffer[i] = padValue;
            }
        }
    }
}
