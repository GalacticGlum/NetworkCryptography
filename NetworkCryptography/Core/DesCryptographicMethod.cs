/*
 * Author: Shon Verch
 * File Name: DesCryptographicMethod.cs
 * Project Name: NetworkCryptography
 * Creation Date: 9/19/2017
 * Modified Date: 10/21/2017
 * Description: An implementation of the DES cryptographic method.
 */

using System;
using System.Collections;
using NetworkCryptography.Core.DataStructures;
using NetworkCryptography.Core.Helpers;

namespace NetworkCryptography.Core
{
    /// Permutation constants (and explanations) are taken from https://en.wikipedia.org/wiki/DES_supplementary_material

    /// <summary>
    /// Data Encryption Standard cryptography method.
    /// </summary>
    public class DesCryptographicMethod : FiestelCryptographicMethod
    {
        /// <summary>
        /// The amount of rounds used in DES.
        /// </summary>
        public new const int Rounds = 16;
        
        /// <summary>
        /// The size of a block in DES.
        /// </summary>
        public new const int BlockSize = 64;

        /// <summary>
        /// Size of a key block.
        /// </summary>
        private const int KeyBlockSize = 56;

        /// <summary>
        /// This table specifies the input permutation on a 64-bit block.
        /// </summary>
        private static readonly Permutation InitialPermuation = new[]
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
        private static readonly Permutation FinalPermuation = new[]
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
        private static readonly Permutation ExpansionFunction = new[]
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
        /// The P permutation table shuffles the bits of a 32-bit half-block.
        /// </summary>
        private static readonly Permutation RoundPermutation = new[]
        {
            16, 7, 20, 21, 29, 12, 28, 17,
            1, 15, 23, 26, 5, 18, 31, 10,
            2, 8 , 24, 14, 32, 27, 3, 9,
            19, 13, 30, 6, 22, 11, 4, 25
        };

        /// <summary>
        /// The first permuted choice table.
        /// </summary>
        private static readonly Permutation PermuationChoice1 = new[]
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
        private static readonly Permutation PermutedChoice2 = new[]
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

        ///// <summary>
        ///// Substitution boxes.
        ///// </summary>
        private static readonly Substitution[] SubstitutionBoxes =
        {
            // Substitution box 1 (S1)
            new[]
            {
                new byte[] { 14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7 },
                new byte[] { 0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8 },
                new byte[] { 4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0 },
                new byte[] { 15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13 }
            },

            // Substitution box 2 (S2)
            new[]
            {
                new byte[] { 15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10 },
                new byte[] { 3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5 },
                new byte[] { 0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15 },
                new byte[] { 13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9 }
            },

            // Substitution box 3 (S3)
            new[]
            {
                new byte[] { 10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8 },
                new byte[] { 13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1 },
                new byte[] { 13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7 },
                new byte[] { 1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 12 }
            },

            // Substitution box 4 (S4)
            new[]
            {
                new byte[] { 7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15 },
                new byte[] { 13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9 },
                new byte[] { 10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4 },
                new byte[] { 3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 5, 11, 12, 7, 2, 14 }
            },

            // Substitution box 5 (S5)
            new[]
            {
                new byte[] { 2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9 },
                new byte[] { 14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6 },
                new byte[] { 4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14 },
                new byte[] { 11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3 }
            },

            // Substitution box 6 (S6)
            new[]
            {
                new byte[] { 12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11 },
                new byte[] { 10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8 },
                new byte[] { 9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6 },
                new byte[] { 4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13 }
            },

            // Substitution box 7 (S7)
            new[]
            {
                new byte[] { 4, 11, 2, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1 },
                new byte[] { 13, 0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8, 6 },
                new byte[] { 1, 4, 11, 13, 12, 3, 7, 14, 10, 15, 6, 8, 0, 5, 9, 2 },
                new byte[] { 6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0, 15, 14, 2, 3, 12 }
            },

            // Substitution box 8 (S8)
            new[]
            {
                new byte[] { 13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7 },
                new byte[] { 1, 15, 13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9, 2 },
                new byte[] { 7, 11, 4, 1, 9, 12, 14, 2, 0, 6, 10, 13, 15, 3, 5, 8 },
                new byte[] { 2, 1, 14, 7, 4, 10, 8, 13, 15, 12, 9, 0, 3, 5, 6, 11 }
            }
        };

        /// <summary>
        /// The round left shifts.
        /// </summary>
        private static int[] LeftShifts =
        {
            1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1
        };

        /// <summary>
        /// Initializes a <see cref="DesCryptographicMethod"/> with a specified <paramref name="keys"/>.
        /// </summary>
        /// <param name="keys"></param>
        public DesCryptographicMethod(byte[] keys) : base(keys, Rounds, BlockSize)
        {          
        }

        /// <summary>
        /// Perform the initial permutation on the <paramref name="cipherText"/>.
        /// </summary>
        /// <param name="cipherText"></param>
        protected override void InitialPermutation(BitSet cipherText) => InitialPermuation.Permute(cipherText);

        /// <summary>
        /// Perform the final permutation on the <paramref name="cipherText"/>.
        /// </summary>
        /// <param name="cipherText"></param>
        protected override void FinalPermutation(BitSet cipherText) => FinalPermuation.Permute(cipherText);

        /// <summary>
        /// Perform a DES round.
        /// </summary>
        /// <param name="right"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        protected override BitSet Round(BitSet right, BitSet key)
        {
            BitSet roundBuffer = (BitSet)right.Clone();
            ExpansionFunction.Permute(roundBuffer);

            roundBuffer ^= key;
            BitSet substitutionBuffer = new BitSet(BlockSize / 2);
            for (int i = 0; i < SubstitutionBoxes.Length; i++)
            { 
                int start = i * Substitution.InputBlockSize;
                BitSet bitsToSubstitute = roundBuffer.GetBits(start, start + Substitution.InputBlockSize);
                SubstitutionBoxes[i].Substitute(bitsToSubstitute);

                substitutionBuffer.CopyFrom(i * Substitution.OutputBlockSize, bitsToSubstitute, bitsToSubstitute.Count);
            }

            RoundPermutation.Permute(roundBuffer);
            return roundBuffer;
        }

        /// <summary>
        /// Calculates the encryption key for a specified round.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="round"></param>
        /// <returns></returns>
        protected override BitSet GetEncryptionKey(BitSet key, int round)
        {
            const int halfKeyBlockSize = KeyBlockSize / 2;
            return GetRoundKey(key, round, (leftBuffer, rightBuffer) =>
            {
                switch (round)
                {
                    case 0:
                    case 1:
                    case 8:
                    case 15:
                        leftBuffer.LeftCircularShft(1, halfKeyBlockSize);
                        rightBuffer.LeftCircularShft(1, halfKeyBlockSize);
                        break;
                    default:
                        leftBuffer.LeftCircularShft(2, halfKeyBlockSize);
                        leftBuffer.LeftCircularShft(2, halfKeyBlockSize);
                        break;
                }
            });
        }

        /// <summary>
        /// Calculates the decryption key for a specified round.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="round"></param>
        /// <returns></returns>
        protected override BitSet GetDecryptionKey(BitSet key, int round)
        {
            const int halfKeyBlockSize = KeyBlockSize / 2;
            return GetRoundKey(key, round, (leftBuffer, rightBuffer) =>
            {
                switch (round)
                {
                    case 0:
                        // Do nothing if it's the 0-th round.
                        break;
                    case 1:
                    case 8:
                    case 15:
                        leftBuffer.RightCircularShft(1, halfKeyBlockSize);
                        rightBuffer.RightCircularShft(1, halfKeyBlockSize);
                        break;
                    default:
                        leftBuffer.RightCircularShft(2, halfKeyBlockSize);
                        leftBuffer.RightCircularShft(2, halfKeyBlockSize);
                        break;
                }
            });
        }

        /// <summary>
        /// Calculates a key for a specific round with a specified rotation function.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="round"></param>
        /// <param name="rotate"></param>
        /// <returns></returns>
        private static BitSet GetRoundKey(BitSet key, int round, Action<BitSet, BitSet> rotate)
        {
            if (round == 0)
            {
                PermuationChoice1.Permute(key);
            }

            const int halfKeyBlockSize = KeyBlockSize / 2;
            BitSet leftBuffer = key.GetBits(0, halfKeyBlockSize);
            BitSet rightBuffer = key.GetBits(halfKeyBlockSize, KeyBlockSize);

            rotate(leftBuffer, rightBuffer);

            key.CopyFrom(0, leftBuffer, halfKeyBlockSize);
            key.CopyFrom(halfKeyBlockSize, rightBuffer, halfKeyBlockSize);

            BitSet roundKey = (BitSet)key.Clone();
            PermutedChoice2.Permute(roundKey);

            return roundKey;
        }
    }
}
