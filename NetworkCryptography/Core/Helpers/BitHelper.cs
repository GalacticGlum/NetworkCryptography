/*
 * Author: Shon Verch
 * File Name: BitHelpoer.cs
 * Project Name: NetworkCryptography
 * Creation Date: 9/19/2017
 * Modified Date: 10/22/2017
 * Description: Helper methods for bit manipulation and related functionality.
 */

using System;

namespace NetworkCryptography.Core.Helpers
{
    public static class BitHelper
    {
        /// <summary>
        /// A bit-mask which represents the left half of a 64-bit value.
        /// </summary>
        private const ulong LeftHalf64Mask = 0xFFFFFFF000000000;

        /// <summary>
        /// Permute a <paramref name="value"/> using a permutation table specifiying the permutation changes. 
        /// </summary>
        /// <param name="value">The value to permute.</param>
        /// <param name="permutationTable">The permutation table specifying the permutation changes.</param>
        /// <returns></returns>
        public static ulong Permute(ulong value, int[] permutationTable)
        {
            const int ulongBitSize = sizeof(ulong) * 8;
            ulong result = 0;

            for (int i = 0; i < permutationTable.Length; i++)
            {
                ulong bit = value >> ulongBitSize - permutationTable[i] & 1;
                result |= bit << ulongBitSize - i - 1;
            }

            return result;
        }

        /// <summary>
        /// Retrieves the left half of a 56-bit block.
        /// </summary>
        /// <param name="block">The 56-bit block.</param>
        /// <returns>The 28-bit left half of the <paramref name="block"/>.</returns>
        public static ulong GetLeftHalfOf56Block(ulong block) => block & LeftHalf64Mask;

        /// <summary>
        /// Retrieves the right half of a 56-bit block.
        /// </summary>
        /// <param name="block">The 56-bit block.</param>
        /// <returns>The 28-bit left half of the <paramref name="block"/>.</returns>
        public static ulong GetRightHalfOf56Block(ulong block) => (block << 28) & LeftHalf64Mask;

        /// <summary>
        /// Join two 56-bit values into one 56-bit values.
        /// </summary>
        /// <param name="left">The 56-bit value which will have the it's first 28-bits be used as the left half of the new joined value.</param>
        /// <param name="right">The 56-bit value which will have the it's first 28-bits be used as the right half of the new joined value.</param>
        /// <returns>The joined 56-bit value.</returns>
        public static ulong JoinHalvesInto56(ulong left, ulong right) => (left & LeftHalf64Mask) | ((right & LeftHalf64Mask) >> 28);

        /// <summary>
        /// Perform a left shift on a 56-bit value.
        /// </summary>
        /// <param name="value">The value to perform the left shift on.</param>
        /// <param name="count">The amount to shift.</param>
        /// <returns>The shifted value.</returns>
        public static ulong LeftShift56(ulong value, int count)
        {
            for (int i = 0; i < count; i++)
            {
                // Get the most significant bit.
                ulong msb = value & 0x8000000000000000;
                value = (value << 1) & 0xFFFFFFE000000000 | msb >> 27;
            }

            return value;
        }

        /// <summary>
        /// Split a 48-bit value into 8 6-bit values.
        /// </summary>
        /// <param name="value">The 48-bit value.</param>
        /// <returns>An array containing the 8 6-bit values.</returns>
        public static byte[] Split48(ulong value)
        {
            byte[] result = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                result[i] = (byte)((value & 0xFC00000000000000) >> 56);
                value <<= 6;
            }

            return result;
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
