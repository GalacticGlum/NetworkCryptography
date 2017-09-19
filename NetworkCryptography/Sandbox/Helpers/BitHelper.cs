/*
 * Author: Shon Verch
 * File Name: BitHelpoer.cs
 * Project Name: NetworkCryptography
 * Creation Date: 9/19/2017
 * Modified Date: 9/19/2017
 * Description: Helper methods for bit manipulation and binary.
 */

using System;
using System.Collections;
using System.Linq;

namespace Sandbox.Helpers
{
    public static class BitHelper
    {
        /// <summary>
        /// The size of a ulong in destination.
        /// </summary>
        private const int UlongBitSize = sizeof(ulong) * 8;

        /// <summary>
        /// Permute a value with a set of permutations to apply.
        /// </summary>
        /// <param name="value">The value to permute.</param>
        /// <param name="permutation">The permutations to apply.</param>
        /// <returns>The permuted value.</returns>
        public static ulong Permute(ulong value, int[] permutation)
        {
            ulong result = 0;
            for (int i = 0; i < permutation.Length; i++)
            {
                ulong bit = value >> UlongBitSize - permutation[i] & 1;
                result |= bit << UlongBitSize - i - 1;
            }

            return result;
        }


        /// <summary>
        /// Copies a BitSet into a destination BitSet, starting from the front of the BitSet.
        /// </summary>
        /// <param name="destination">The BitSet to copy into.</param>
        /// <param name="readStartPosition">The position to begin reading, on the source.</param>
        /// <param name="source">The BitSet to copy from.</param>
        /// <param name="copyLength">The number of bits to copy.</param>
        public static void CopyFrom(this BitSet destination, int readStartPosition, BitSet source, int copyLength)
        {
            CopyFrom(destination, readStartPosition, source, 0, copyLength);
        }

        /// <summary>
        /// Copies a BitSet into a destination BitSet, starting from a specified index.
        /// </summary>
        /// <param name="destination">The BitSet to copy into.</param>
        /// <param name="readStartPosition">The position to begin reading, on the source.</param>
        /// <param name="source">The BitSet to copy from.</param>
        /// <param name="initialPosition">The position to begin writing the copied bits, on the destination.</param>
        /// <param name="copyLength">The number of bits to copy.</param>
        public static void CopyFrom(this BitSet destination, int readStartPosition, BitSet source, int initialPosition, int copyLength)
        {
            for (int i = initialPosition, j = readStartPosition; i < initialPosition + copyLength; i++, j++)
            {
                destination.Set(i, source.Get(j));
            }
        }

        /// <summary>
        /// Prints an integer in binary format.
        /// </summary>
        /// <param name="value">The value to print as binary.</param>
        public static void PrintAsBinary(this int value)
        {
            Console.WriteLine(Convert.ToString(value, 2).PadLeft(8, '0'));
        }

        /// <summary>
        /// Prints a byte in binary format.
        /// </summary>
        /// <param name="value">The value to print as binary.</param>
        public static void PrintAsBinary(this byte value)
        {
            Console.WriteLine(Convert.ToString(value, 2).PadLeft(8, '0'));
        }
    }
}
