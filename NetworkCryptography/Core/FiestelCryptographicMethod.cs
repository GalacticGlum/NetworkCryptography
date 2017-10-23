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
using NetworkCryptography.Core.Helpers;

namespace NetworkCryptography.Core
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

        /// <summary>
        /// Random engine.
        /// </summary>
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
        /// <param name="plaintext"></param>
        /// <returns></returns>
        public string Encrypt(string plaintext)
        {
            // Create the block buffer, this will store the contents of the active block. 
            int blockSizeInBytes = BlockSize / (sizeof(byte) * 8);
            byte[] blockBuffer = new byte[blockSizeInBytes];

            using (MemoryStream dataBuffer = new MemoryStream(Encoding.Unicode.GetBytes(plaintext)))
            {
                // Read in the first block of data.
                int bytesRead = dataBuffer.Read(blockBuffer, 0, blockSizeInBytes);

                BitSet keyBuffer = new BitSet(Keys);
                BitSet currentCipherBlock = CreateInitializationVector();

                using (MemoryStream outputBuffer = new MemoryStream())
                {
                    // Write the initialization vector to the output
                    outputBuffer.Write(currentCipherBlock.ToBytesInBigEndian(), 0, blockSizeInBytes);

                    while (bytesRead > 0)
                    {
                        // If we have read less than a block data, we need to pad the block with data.
                        if (bytesRead < blockSizeInBytes)
                        {
                            Pad(blockBuffer, bytesRead, blockSizeInBytes);
                        }

                        // Process our block buffer and encrypt a new block of data.
                        BitSet blockBits = new BitSet(blockBuffer);
                        blockBits ^= currentCipherBlock; 
                        currentCipherBlock = ProcessBlock(blockBits, keyBuffer, GetEncryptionKey);

                        // Write out the encrypted block to the output stream.
                        outputBuffer.Write(currentCipherBlock.ToBytesInBigEndian(0, BlockSize), 0, blockSizeInBytes);

                        // Read in a new block of data
                        bytesRead = dataBuffer.Read(blockBuffer, 0, blockSizeInBytes);
                    }

                    return Encoding.Unicode.GetString(outputBuffer.ToArray());
                }
            }
        }

        /// <summary>
        /// Decrypt a message.
        /// </summary>
        /// <param name="encryptedMessage">The encrypted message.</param>
        /// <returns>The decrypted message.</returns>
        public string Decrypt(string encryptedMessage)
        {
            // Create our block buffer
            int blockSizeInBytes = BlockSize / (sizeof(byte) * 8);
            byte[] blockBuffer = new byte[blockSizeInBytes];

            using (MemoryStream dataBuffer = new MemoryStream(Encoding.Unicode.GetBytes(encryptedMessage)))
            {
                // Read the initialization vector
                byte[] initializationVectorBuffer = new byte[blockSizeInBytes];
                dataBuffer.Read(initializationVectorBuffer, 0, blockSizeInBytes);

                BitSet keyBuffer = new BitSet(Keys);
                BitSet currentPlainTextBlock = new BitSet(initializationVectorBuffer);

                // Read the first block of data.
                int bytesRead = dataBuffer.Read(blockBuffer, 0, blockSizeInBytes);
                using (MemoryStream outputBuffer = new MemoryStream())
                {
                    while (bytesRead > 0)
                    {
                        BitSet blockBits = new BitSet(blockBuffer);
                        // Read in our data.
                        bytesRead = dataBuffer.Read(blockBuffer, 0, blockSizeInBytes);

                        // Our bytesAfterUnpadding is equal to the amount of bytes we have read in.
                        // We assume that we don't need unpadding initially.
                        int bytesAfterUnpadding = bytesRead; 

                        // Process our block of data
                        BitSet decryptedBlock = ProcessBlock(blockBits, keyBuffer, GetDecryptionKey);
                        currentPlainTextBlock ^= decryptedBlock;

                        // If we have reached the end of the stream then let's finally unpad our data.
                        if (bytesRead <= 0)
                        {
                            bytesAfterUnpadding = Unpad(currentPlainTextBlock, blockSizeInBytes);
                        }

                        // Write the new block of decrypted data to our output stream
                        outputBuffer.Write(currentPlainTextBlock.ToBytesInBigEndian(0, bytesAfterUnpadding * 8), 0, bytesAfterUnpadding);
                        currentPlainTextBlock = blockBits;
                    }

                    return Encoding.Unicode.GetString(outputBuffer.ToArray());
                }
            }
        }

        /// <summary>
        /// Process a block of data with a specific key function.
        /// </summary>
        /// <param name="block">The block to process.</param>
        /// <param name="key">The key.</param>
        /// <param name="keyFunction">The specific key computation function.</param>
        /// <returns></returns>
        private BitSet ProcessBlock(BitSet block, BitSet key, Func<BitSet, int, BitSet> keyFunction)
        {
            // Create a copy of the data before we begin manipulating it.
            BitSet blockBuffer = (BitSet)block.Clone();
            BitSet keyBuffer = (BitSet) key.Clone();

            // Execute our initial permutation onto the block of data.
            InitialPermutation(blockBuffer);

            // Get the left and right segments of the block.
            BitSet left = blockBuffer.GetBits(0, BlockSize / 2);
            BitSet right = blockBuffer.GetBits(BlockSize / 2, BlockSize);

            for (int i = 0; i < Rounds; i++)
            {
                StepRound(left, right, keyFunction(keyBuffer, i));
            }

            // Copy the manipulated segments back to our block buffer.
            SwapBits(left, right);
            blockBuffer.CopyFrom(0, left, BlockSize / 2);
            blockBuffer.CopyFrom(BlockSize / 2, right, BlockSize / 2);
            
            // Execute our final permutation onto the block of data.
            FinalPermutation(blockBuffer);
            return blockBuffer;
        }

        /// <summary>
        /// Step a round in the Fiestel cipher.
        /// </summary>
        /// <param name="left">The left segment.</param>
        /// <param name="right">The right segment.</param>
        /// <param name="key">The key.</param>
        private void StepRound(BitSet left, BitSet right, BitSet key)
        {
            left ^= Round(right, key);
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
                bool leftValue = left.Get(i);
                left.Set(i, right.Get(i));
                right.Set(i, leftValue);
            }
        }

        /// <summary>
        /// Create the initialization vector.
        /// The initialization vector is just a block filled with random values.
        /// </summary>
        /// <returns></returns>
        private BitSet CreateInitializationVector()
        {
            // An initialization vector is just a block of data filled with random values.
            // It is used to pseudorandomize the ciphertext without having to waste processing
            // on re-keying the cipher.

            byte[] buffer = new byte[BlockSize / (sizeof(byte) * 8)];
            random.NextBytes(buffer);

            return new BitSet(buffer);
        }

        /// <summary>
        /// Pad data to a certain length.
        /// </summary>
        /// <param name="buffer">The byte array to pad.</param>
        /// <param name="dataSize">The size of the data inside the buffer.</param>
        /// <param name="targetSize">The target size of the whole buffer.</param>
        private static void Pad(byte[] buffer, int dataSize, int targetSize)
        {
            // The pad value is the delta size of our CURRENT data and our TARGET data.
            byte padValue = (byte)(targetSize - dataSize);
            for (int i = dataSize; i < targetSize; i++)
            {
                buffer[i] = padValue;
            }
        }

        /// <summary>
        /// Unpad data to a certain length.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="targetSize"></param>
        /// <returns></returns>
        private static int Unpad(BitSet buffer, int targetSize)
        {
            byte[] data = buffer.ToBytesInBigEndian();

            // Since we pad data with the delta size between the current and target data.
            // We can check if data is padded by seeing if the last value is less than
            // our target size, and if it's greater than 0 (since we can't have negative size).
            // Otherwise, we don't have padded data, return our target size.
            if (data[targetSize - 1] >= targetSize || data[targetSize - 1] <= 0) return targetSize;

            int count = 0;   
            byte padValue = data[targetSize - 1];

            for (int i = targetSize - 1; i > 0; i--)
            {
                // If the element at i is our padValue then increment the count.
                if (data[i] == padValue)
                {
                    count++;
                }
            }

            // If our count is equal to our pad value then this block of data is padded.
            // This is because our pad value is the delta size betweem our current and target sizes.
            // Therefore, if our count is equal to the amount of data that was padded, let's 
            // unpad our data. 
            // Otherwise, we don't have padded data, so let's return our target size.
            if (count != padValue) return targetSize;

            byte[] newData = new byte[targetSize - padValue];
            Array.Copy(data, 0, newData, 0, newData.Length);

            BitSet newBuffer = new BitSet(newData);
            buffer.Replace(newBuffer);

            return newData.Length;
        }
    }
}
