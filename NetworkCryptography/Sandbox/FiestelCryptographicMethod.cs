/*
 * Author: Shon Verch
 * File Name: FiestelCryptographicMethod.cs
 * Project Name: NetworkCryptography
 * Creation Date: 9/19/2017
 * Modified Date: 9/20/2017
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
    public abstract class FiestelCryptographicMethod 
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

        /// <summary>
        /// The initial permutation on the ciphertext.
        /// </summary>
        /// <param name="cipherText"></param>
        protected abstract void InitialPermutation(BitSet cipherText);

        /// <summary>
        /// The final permutation on the cipertext.
        /// </summary>
        /// <param name="cipherText"></param>
        protected abstract void FinalPermutation(BitSet cipherText);
       
        /// <summary>
        /// The f(k, r) function in a round.
        /// </summary>
        /// <param name="right"></param>
        /// <param name="key"></param>
        /// <returns>The computed right value.</returns>
        protected abstract BitSet Round(BitSet right, BitSet key);

        /// <summary>
        /// The specific key generation for a round when encrypting.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="round"></param>
        /// <returns>The key.</returns>
        protected abstract BitSet GetEncryptionKey(BitSet key, int round);
        
        /// <summary>
        /// The specific key generation for a round when decrypting.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="round"></param>
        /// <returns></returns>
        protected abstract BitSet GetDecryptionKey(BitSet key, int round);
        
        /// <summary>
        /// Generic constructor for a Feistel cipher.
        /// </summary>
        /// <param name="keys">The keys to be used in this session.</param>
        /// <param name="rounds">The amount of rounds.</param>
        /// <param name="blockSize">The size of a block, in bits.</param>
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
        public byte[] Encrypt(string message)
        {
            // Create the block buffer, this will store the contents of the active block. 
            int blockSizeInBytes = BlockSize / (sizeof(byte) * 8);
            byte[] blockBuffer = new byte[blockSizeInBytes];

            using (MemoryStream dataBuffer = new MemoryStream(Encoding.Unicode.GetBytes(message)))
            {
                int bytesRead = dataBuffer.Read(blockBuffer, 0, blockSizeInBytes);

                BitSet keyBuffer = new BitSet(Keys);
                BitSet currentCipherBlock = CreateInitializationVector();

                using (MemoryStream outputBuffer = new MemoryStream())
                {
                    // Write the initialization vector to the output
                    outputBuffer.Write(currentCipherBlock.ToBytes(), 0, blockSizeInBytes);

                    while (bytesRead > 0)
                    {
                        if (bytesRead < blockSizeInBytes)
                        {
                            Pad(blockBuffer, bytesRead, blockSizeInBytes);
                        }

                        BitSet blockBits = new BitSet(blockBuffer);
                        blockBits.Xor(currentCipherBlock);
                        currentCipherBlock = EncryptBlock(blockBits, keyBuffer);

                        outputBuffer.Write(currentCipherBlock.ToBytes(0, BlockSize), 0, blockSizeInBytes);
                        bytesRead = dataBuffer.Read(blockBuffer, 0, blockSizeInBytes);
                    }

                    return outputBuffer.ToArray();
                }
            }
        }

        public string Decrypt(byte[] encryptedMessage)
        {
            // Create our block buffer
            int blockSizeInBytes = BlockSize / (sizeof(byte) * 8);
            byte[] blockBuffer = new byte[blockSizeInBytes];

            using (MemoryStream dataBuffer = new MemoryStream(encryptedMessage))
            {
                // Read Initialization vector
                byte[] initializationVectorBuffer = new byte[blockSizeInBytes];
                dataBuffer.Read(initializationVectorBuffer, 0, blockSizeInBytes);

                BitSet keyBuffer = new BitSet(Keys);
                BitSet currentPlainTextBlock = new BitSet(initializationVectorBuffer);

                int bytesRead = dataBuffer.Read(blockBuffer, 0, blockSizeInBytes);
                using (MemoryStream outputBuffer = new MemoryStream())
                {
                    while (bytesRead > 0)
                    {
                        BitSet blockBits = new BitSet(blockBuffer);
                        int bytesAfterUnpadding = bytesRead = dataBuffer.Read(blockBuffer, 0, blockSizeInBytes);

                        BitSet decryptedBlock = DecryptBlock(blockBits, keyBuffer);
                        currentPlainTextBlock.Xor(decryptedBlock);
                        if (bytesRead <= 0)
                        {
                            bytesAfterUnpadding = Unpad(currentPlainTextBlock, blockSizeInBytes);
                        }

                        outputBuffer.Write(currentPlainTextBlock.ToBytes(0, bytesAfterUnpadding * 8), 0, bytesAfterUnpadding);
                        currentPlainTextBlock = blockBits;
                    }
 
                    return Encoding.Unicode.GetString(outputBuffer.ToArray());
                }
            }
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
        /// Decrypt a block of data.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private BitSet DecryptBlock(BitSet block, BitSet key)
        {
            BitSet plainTextBuffer = (BitSet) block.Clone();
            BitSet keyBuffer = (BitSet) key.Clone();

            InitialPermutation(plainTextBuffer);

            BitSet left = plainTextBuffer.GetBits(0, BlockSize / 2);
            BitSet right = plainTextBuffer.GetBits(BlockSize / 2, BlockSize);

            for (int i = 0; i < Rounds; i++)
            {
                StepRound(left, right, GetDecryptionKey(keyBuffer, i));
            }

            SwapBits(left, right);
            plainTextBuffer.CopyFrom(0, left, BlockSize / 2);
            plainTextBuffer.CopyFrom(BlockSize / 2, right, BlockSize / 2);

            FinalPermutation(plainTextBuffer);
            return plainTextBuffer;
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
            byte[] buffer = new byte[BlockSize / (sizeof(byte) * 8)];
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

        private static int Unpad(BitSet buffer, int targetSize)
        {
            byte[] data = buffer.ToBytes();
            if (data[targetSize - 1] < targetSize && data[targetSize - 1] > 0)
            {
                byte n = data[targetSize - 1];
                int count = 0;
                
                for (int i = targetSize - 1; i > 0; i--)
                {
                    if (data[i] == n)
                    {
                        count++;
                    }
                }

                if (count == n)
                {
                    byte[] newData = new byte[targetSize - n];
                    Array.Copy(data, 0, newData, 0, newData.Length);

                    BitSet newBuffer = new BitSet(newData);
                    buffer.Replace(newBuffer);

                    return newData.Length;
                }
            }

            return targetSize;
        }
    }
}
