/*
 * Author: Shon Verch
 * File Name: BitHelpoer.cs
 * Project Name: NetworkCryptography
 * Creation Date: 9/19/2017
 * Modified Date: 9/28/2017
 * Description: Helper methods for bit manipulation and binary.
 */

using System;
using System.Collections;

namespace NetworkCryptography.Core.Helpers
{
    public static class BitHelper
    {
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
                destination.Set(j, source.Get(i));
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
