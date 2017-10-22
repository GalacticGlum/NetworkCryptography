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
        /// <param name="startWritePosition">The position to begin reading, on the source.</param>
        /// <param name="source">The BitSet to copy from.</param>
        /// <param name="copyLength">The number of bits to copy.</param>
        public static void CopyFrom(this BitSet destination, int startWritePosition, BitSet source, int copyLength)
        {
            CopyFrom(destination, startWritePosition, source, 0, copyLength);
        }

        /// <summary>
        /// Copies a BitSet into a destination BitSet, starting from a specified index.
        /// </summary>
        /// <param name="destination">The BitSet to copy into.</param>
        /// <param name="startWritePosition">The position to begin writing the copied bits, on the destination.</param>
        /// <param name="source">The BitSet to copy from.</param>
        /// <param name="startReadPosition">The position to begin reading, on the source.</param>
        /// <param name="copyLength">The number of bits to copy.</param>
        public static void CopyFrom(this BitSet destination, int startWritePosition, BitSet source, int startReadPosition, int copyLength)
        {
            for (int i = 0; i < copyLength; i++)
            {
                destination.Set(i + startWritePosition, source.Get(i + startReadPosition));
            }
        }

        /// <summary>
        /// Shift the <see cref="BitSet"/> circularly in the right direction.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        public static void RightCircularShft(this BitSet buffer, int offset, int size)
        {
            BitSet leftBuffer = buffer.GetBits(0, size - offset);
            BitSet rightBuffer = buffer.GetBits(size - offset, size);

            buffer.CopyFrom(0, rightBuffer, offset);
            buffer.CopyFrom(offset, leftBuffer, size - offset);
        }


        /// <summary>
        /// Shift the <see cref="BitSet"/> circularly in the left direction.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        public static void LeftCircularShft(this BitSet buffer, int offset, int size)
        {
            BitSet leftBuffer = buffer.GetBits(0, offset);
            BitSet rightBuffer = buffer.GetBits(offset, size);

            buffer.CopyFrom(0, rightBuffer, size - offset);
            buffer.CopyFrom(size - offset, leftBuffer, offset);
        }

        /// <summary>
        /// Converts a <see cref="BitSet"/> to a <see cref="byte"/> array in Big-Endian order.
        /// </summary>
        /// <param name="buffer">The <see cref="BitSet"/> to convert.</param>
        /// <param name="start">The index of the bit to start converting.</param>
        /// <param name="conversionLength">The amount of bits to convert.</param>
        public static byte[] ToBytesInBigEndian(this BitSet buffer, int start = 0, int conversionLength = -1)
        {
            if (conversionLength <= 0)
            {
                conversionLength = buffer.Count / 8 - start;
            }

            if (buffer.Count % 8 != 0) conversionLength++;

            byte[] bytes = new byte[conversionLength];

            int indexOfByte = 0;
            int indexOfBit = 0;

            for (int i = 0; i < buffer.Count; i++)
            {
                if (buffer.Get(i))
                {
                    bytes[indexOfByte] |= (byte)(1 << indexOfBit);
                }

                indexOfBit++;
                if (indexOfBit != 8) continue;

                indexOfBit = 0;
                indexOfByte++;
            }

            return bytes;
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
