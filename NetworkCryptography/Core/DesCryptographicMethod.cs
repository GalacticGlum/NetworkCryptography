/*
 * Author: Shon Verch
 * File Name: DesCryptographicMethod.cs
 * Project Name: NetworkCryptography
 * Creation Date: 9/19/2017
 * Modified Date: 10/22/2017
 * Description: An implementation of the DES cryptographic method.
 */

using System;
using NetworkCryptography.Core.DataStructures;
using NetworkCryptography.Core.Helpers;

namespace NetworkCryptography.Core
{
    /// <summary>
    /// Data Encryption Standard cryptography method.
    /// </summary>
    public class DesCryptographicMethod : ICryptographicMethod
    {
        /// <summary>
        /// Represents two halves of a data block. 
        /// This is only used internally inside the <see cref="DesCryptographicMethod"/>.
        /// </summary>
        private struct Block
        {
            /// <summary>
            /// The left half of the block.
            /// </summary>
            public ulong Left { get; set; }

            /// <summary>
            /// The right half of the block.
            /// </summary>
            public ulong Right { get; set; }

            /// <summary>
            /// Initializes a <see cref="Block"/> with left and right values.
            /// </summary>
            /// <param name="left"></param>
            /// <param name="right"></param>
            public Block(ulong left, ulong right)
            {
                Left = left;
                Right = right;
            }
        }

        /// <summary>
        /// Amount of rounds to perform.
        /// </summary>
        public const int Rounds = 16;

        /// <summary>
        /// Initial permuation table.
        /// </summary>
        private static readonly int[] InitialPermutation =
        {
            58, 50, 42, 34, 26, 18, 10, 2,
            60, 52, 44, 36, 28, 20, 12, 4,
            62, 54, 46, 38, 30, 22, 14, 6,
            64, 56, 48, 40, 32, 24, 16, 8,
            57, 49, 41, 33, 25, 17, 9, 1,
            59, 51, 43, 35, 27, 19, 11, 3,
            61, 53, 45, 37, 29, 21, 13, 5,
            63, 55, 47, 39, 31, 23, 15, 7
        };

        /// <summary>
        /// Final permutation table. This is the inverse of the initial permutation table.
        /// </summary>
        private static readonly int[] FinalPermutation =
        {
            40, 8, 48, 16, 56, 24, 64, 32,
            39, 7, 47, 15, 55, 23, 63, 31,
            38, 6, 46, 14, 54, 22, 62, 30,
            37, 5, 45, 13, 53, 21, 61, 29,
            36, 4, 44, 12, 52, 20, 60, 28,
            35, 3, 43, 11, 51, 19, 59, 27,
            34, 2, 42, 10, 50, 18, 58, 26,
            33, 1, 41, 9, 49, 17, 57, 25
        };

        /// <summary>
        /// The expansion function table.
        /// </summary>
        private static readonly int[] Expansion =
        {
            32, 1, 2, 3, 4, 5,
            4, 5, 6, 7, 8, 9,
            8, 9, 10, 11, 12, 13,
            12, 13, 14, 15, 16, 17,
            16, 17, 18, 19, 20, 21,
            20, 21, 22, 23, 24, 25,
            24, 25, 26, 27, 28, 29,
            28, 29, 30, 31, 32, 1
        };

        /// <summary>
        /// Permutation table.
        /// </summary>
        private static readonly int[] ShufflePermutation =
        {
            16, 7, 20, 21,
            29, 12, 28, 17,
            1, 15, 23, 26,
            5, 18, 31, 10,
            2, 8, 24, 14,
            32, 27, 3, 9,
            19, 13, 30, 6,
            22, 11, 4, 25
        };

        /// <summary>
        /// The first permuted choice table.
        /// </summary>
        private static readonly int[] PermutationChoice1 =
        {
            57, 49, 41, 33, 25, 17, 9,
            1, 58, 50, 42, 34, 26, 18,
            10, 2, 59, 51, 43, 35, 27,
            19, 11, 3, 60, 52, 44, 36,
            63, 55, 47, 39, 31, 23, 15,
            7, 62, 54, 46, 38, 30, 22,
            14, 6, 61, 53, 45, 37, 29,
            21, 13, 5, 28, 20, 12, 4
        };

        /// <summary>
        /// The second permuted choice table.
        /// </summary>
        private static readonly int[] PermutedChoice2 =
        {
            14, 17, 11, 24, 1, 5,
            3, 28, 15, 6, 21, 10,
            23, 19, 12, 4, 26, 8,
            16, 7, 27, 20, 13, 2,
            41, 52, 31, 37, 47, 55,
            30, 40, 51, 45, 33, 48,
            44, 49, 39, 56, 34, 53,
            46, 42, 50, 36, 29, 32
        };

        /// <summary>
        /// Substitution boxes.
        /// </summary>
        private static readonly byte[,] SubstitutionBoxes =
        {
            // Substitution box 1.
            {
                14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7,
                0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8,
                4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0,
                15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13
            },

            // Substitution box 2.
            {
                15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10,
                3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5,
                0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15,
                13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9
            },

            // Substitution box 3.
            {
                10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8,
                13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1,
                13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7,
                1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 12
            },

            // Substitution box 4.
            {
                7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15,
                13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9,
                10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4,
                3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 5, 11, 12, 7, 2, 14
            },

            // Substitution box 5.
            {
                2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9,
                14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6,
                4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14,
                11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3
            },

            // Substitution box 6.
            {
                12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11,
                10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8,
                9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6,
                4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13
            },

            // Substitution box 7.
            {
                4, 11, 2, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1,
                13, 0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8, 6,
                1, 4, 11, 13, 12, 3, 7, 14, 10, 15, 6, 8, 0, 5, 9, 2,
                6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0, 15, 14, 2, 3, 12
            },

            // Substitution box 8.
            {
                13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7,
                1, 15, 13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9, 2,
                7, 11, 4, 1, 9, 12, 14, 2, 0, 6, 10, 13, 15, 3, 5, 8,
                2, 1, 14, 7, 4, 10, 8, 13, 15, 12, 9, 0, 3, 5, 6, 11
            }
        };

        /// <summary>
        /// The round left shifts.
        /// </summary>
        private static readonly int[] KeyRotations = { 1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1 };

        /// <summary>
        /// The key to use for encryption and decryption.
        /// </summary>
        public ulong Key { get; }

        /// <summary>
        /// The random service provider.
        /// </summary>
        private readonly Random random = new Random();

        /// <summary>
        /// Initializes a new <see cref="DesCryptographicMethod"/> with a key.
        /// </summary>
        /// <param name="key"></param>
        public DesCryptographicMethod(ulong key)
        {
            Key = key;
        }

        /// <summary>
        /// Encrypt plaintext.
        /// </summary>
        /// <param name="plaintext">The plaintext to encrypt.</param>
        /// <returns>A <see cref="PaddedBuffer"/> representing the ciphertext.</returns>
        public byte[] Encrypt(string plaintext)
        {
            return EncryptBlocks(new PaddedBuffer(plaintext)).ToBytes();
        }

        /// <summary>
        /// Encrypt a <see cref="PaddedBuffer"/> representing blocks of plaintext.
        /// </summary>
        /// <param name="plaintext">The <see cref="PaddedBuffer"/> of blocks.</param>
        /// <returns>A <see cref="PaddedBuffer"/> representing the ciphertext.</returns>
        private PaddedBuffer EncryptBlocks(PaddedBuffer plaintext)
        {
            // Initialize the input buffer and copy the data from plaintext into it.
            BlockBuffer inputBuffer = new BlockBuffer(plaintext.Count + 1);
            inputBuffer.BlockCopy(plaintext.Data, 0, 1, plaintext.Count);

            // Generate a random block of data to be used as the first block.
            // Since we don't know the initialization vector on decryption, the first
            // block of data will be corrupted. In order to circumvent this,
            // if our first block of data is one that is irrelevant then it doesn't matter.
            inputBuffer.Set(0, CryptographyHelper.GenerateRandomBlock(BlockBuffer.BlockSize));

            BlockBuffer outputBuffer = new BlockBuffer(inputBuffer.Count);
            ulong currentBlock = CryptographyHelper.GenerateRandomBlock(BlockBuffer.BlockSize);
            for (int i = 0; i < inputBuffer.Count; i++)
            {
                ulong block = inputBuffer[i];
                block ^= currentBlock;

                currentBlock = Cipher(block, true);
                outputBuffer[i] = currentBlock;
            }

            plaintext.Data = outputBuffer;
            return plaintext;
        }

        /// <summary>
        /// Decrypt ciphertext.
        /// </summary>
        /// <param name="ciphertext">The ciphertext to decrypt.</param>
        /// <returns>A string representing the plaintext.</returns>
        public string Decrypt(byte[] ciphertext)
        {
            PaddedBuffer buffer = new PaddedBuffer(ciphertext);
            return DecryptBlocks(buffer).ToString();
        }

        /// <summary>
        /// Decrypt a <see cref="PaddedBuffer"/> representing blocks of ciphertext.
        /// </summary>
        /// <param name="ciphertext">The ciphertext to decrypt.</param>
        /// <returns>A <see cref="PaddedBuffer"/> representing the plaintext.</returns>
        private PaddedBuffer DecryptBlocks(PaddedBuffer ciphertext)
        {
            BlockBuffer outputBuffer = new BlockBuffer(ciphertext.Count - 1);
            ulong currentBlock = ciphertext[0];

            for (int i = 1; i < ciphertext.Count; i++)
            {
                ulong block = ciphertext[i];
                ulong decoded = Cipher(block, false);
                ulong plaintextBuffer = currentBlock ^ decoded;

                currentBlock = block;
                outputBuffer[i - 1] = plaintextBuffer;
            }

            ciphertext.Data = outputBuffer;
            return ciphertext;
        }

        /// <summary>
        /// Perform a DES cipher.
        /// </summary>
        /// <param name="block">The block of data to perform the cipher on.</param>
        /// <param name="isEncrypting">Indicates whether we should encrypt or decrypt the block of data.</param>
        /// <returns></returns>
        private ulong Cipher(ulong block, bool isEncrypting)
        {
            ulong initialPermutation = BitHelper.Permute(block, InitialPermutation);
            var schedule = GetKeySchedule(Key);

            const ulong leftHalfMask = 0xFFFFFFFF00000000;
            const ulong rightHalfMask = 0x00000000FFFFFFFF;

            Block currentBlock = new Block
            {
                Left = initialPermutation & leftHalfMask,
                Right = (initialPermutation & rightHalfMask) << 32
            };

            if (isEncrypting)
            {
                for (int i = 0; i < Rounds; i++)
                {
                    currentBlock = StepRound(schedule[i + 1], currentBlock);
                }
            }
            else
            {
                // When we are decrypting, we want to go the opposite direction in blocks.
                for (int i = Rounds - 1; i >= 0; i--)
                {
                    currentBlock = StepRound(schedule[i + 1], currentBlock);
                }
            }

            ulong joined = currentBlock.Right | (currentBlock.Left >> 32);
            return BitHelper.Permute(joined, FinalPermutation);
        }

        /// <summary>
        /// Perform an individual DES round.
        /// </summary>
        /// <param name="roundKey">The key to use for tihs round.</param>
        /// <param name="block">The block of data.</param>
        /// <returns>The block of data after this round.</returns>
        private Block StepRound(ulong roundKey, Block block)
        {
            return new Block
            {
                Left = block.Right,
                Right = block.Left ^ Round(block.Right, roundKey)
            };
        }

        /// <summary>
        /// The f(k, r): round calulation function based on the right half of a block and a key.
        /// </summary>
        /// <param name="right">The right half of a block.</param>
        /// <param name="roundKey">The key to use for this round.</param>
        /// <returns>The right half of data after round calculation.</returns>
        public ulong Round(ulong right, ulong roundKey)
        {
            ulong expansion = BitHelper.Permute(right, Expansion);
            ulong data = expansion ^ roundKey;

            // Split our data into 8 6-bit values to make the substitution easier.
            byte[] bytes = BitHelper.Split48(data);
            ulong substitutionBox = 0;

            // For each substitution box, perform a lookup; there are 8 boxes.
            for (int i = 0; i < 8; i++)
            {
                substitutionBox <<= 4;
                substitutionBox |= LookupSubstitutionBox(bytes[i], i);
            }

            substitutionBox <<= 32;
            return BitHelper.Permute(substitutionBox, ShufflePermutation);
        }

        /// <summary>
        /// Retrieves the key schedule for a key.
        /// </summary>
        /// <param name="key">The key to retrieve the key schedule for.</param>
        /// <returns>The key schedule.</returns>
        public ulong[] GetKeySchedule(ulong key)
        {
            ulong permutation = BitHelper.Permute(key, PermutationChoice1);
            ulong left = BitHelper.GetLeftHalfOf56Block(permutation);
            ulong right = BitHelper.GetRightHalfOf56Block(permutation);

            // Initialize our schedule so we have a block to "work with."
            Block[] schedule = new Block[KeyRotations.Length + 1];
            schedule[0] = new Block { Left = left, Right = right };

            for (int i = 1; i <= KeyRotations.Length; i++)
            {
                schedule[i] = new Block
                {
                    Left = BitHelper.LeftShift56(schedule[i - 1].Left, KeyRotations[i - 1]),
                    Right = BitHelper.LeftShift56(schedule[i - 1].Right, KeyRotations[i - 1])
                };
            }

            // Join our key schedules into final values and permute them by the second permutation choice.
            ulong[] result = new ulong[schedule.Length];
            for (int i = 0; i < schedule.Length; i++)
            {
                ulong joined = BitHelper.JoinHalvesInto56(schedule[i].Left, schedule[i].Right);
                ulong permuted = BitHelper.Permute(joined, PermutedChoice2);

                result[i] = permuted;
            }

            return result;
        }

        /// <summary>
        /// Lookup a substitution box based on a value and table index.
        /// </summary>
        /// <param name="value">The value used for substitution.</param>
        /// <param name="table">The index of the substitution table.</param>
        /// <returns></returns>
        public static byte LookupSubstitutionBox(byte value, int table)
        {
            int index = ((value & 0x80) >> 2) | ((value & 0x04) << 2) | ((value & 0x78) >> 3);
            return SubstitutionBoxes[table, index];
        }
    }
}
