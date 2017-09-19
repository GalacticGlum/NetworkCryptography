/*
 * Author: Shon Verch
 * File Name: BitHelpoer.cs
 * Project Name: NetworkCryptography
 * Creation Date: 9/19/2017
 * Modified Date: 9/19/2017
 * Description: Helper methods for bit manipulation and binary.
 */

using System;

namespace Sandbox.Helpers
{
    public static class BitHelper
    {
        /// <summary>
        /// The size of a ulong in bits.
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
        /// Prints an integer in binary format.
        /// </summary>
        /// <param name="value"></param>
        public static void PrintAsBinary(this int value)
        {
            Console.WriteLine(Convert.ToString(value, 2).PadLeft(8, '0'));
        }

        /// <summary>
        /// Prints a byte in binary format.
        /// </summary>
        /// <param name="value"></param>
        public static void PrintAsBinary(this byte value)
        {
            Console.WriteLine(Convert.ToString(value, 2).PadLeft(8, '0'));
        }
    }
}
