/*
 * Author: Shon Verch
 * File Name: Permutation.cs
 * Project Name: NetworkCryptography
 * Creation Date: 10/21/2017
 * Modified Date: 10/21/2017
 * Description: A data structure which stores the substitution values for a DES substitution sequence.
 */

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NetworkCryptography.Core.Helpers;

namespace NetworkCryptography.Core.DataStructures
{
    /// <summary>
    /// A data structure which stores the substitution values for a DES substitution sequence.
    /// </summary>
    public class Substitution : IEnumerable<BitSet[]>
    {
        /// <summary>
        /// The size of a substitution box input in bits.
        /// </summary>
        public const int InputBlockSize = 6;

        /// <summary>
        /// The size of an output from a substitution box in bits.
        /// </summary>
        public const int OutputBlockSize = 4;

        /// <summary>
        /// Retrieves a clone of the value at the <paramref name="x"/> and <paramref name="y"/> position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public BitSet this[int x, int y] => Get(x, y);

        /// <summary>
        /// The substitution table.
        /// </summary>
        private readonly BitSet[][] values;

        /// <summary>
        ///  Initializes a <see cref="Substitution"/> with a jagged array of <see cref="byte"/> array of substitution values.
        /// </summary>
        /// <param name="substitutionValues">The jagged <see cref="byte"/> array of substitution values.</param>
        public Substitution(byte[][] substitutionValues)
        {
            values = BytesToBitSets(substitutionValues);
        }

        /// <summary>
        ///  Initializes a <see cref="Substitution"/> with a jagged array of <see cref="BitSet"/> array of substitution values.
        /// </summary>
        /// <param name="substitutionValues">The jagged <see cref="BitSet"/> array of substitution values.</param>
        public Substitution(BitSet[][] substitutionValues)
        {
            values = substitutionValues;
        }

        /// <summary>
        /// Substitute a <see cref="BitSet"/> for a specified subsitution value.
        /// </summary>
        /// <param name="x">The x-position of the subsitution value.</param>
        /// <param name="y">The y-position of the substituion value.</param>
        /// <param name="bits">The <see cref="BitSet"/> to substitute into.</param>
        public void Substitute(int x, int y, BitSet bits) => bits.Replace(Get(x, y));

        public void Substitute(BitSet bits)
        {
            // Initialize our temporary vector, this is simply used to convert bits to bytes.
            BitSet temporary = new BitSet(2);

            // Calculate the y-position of the substitution.
            // We take the first bit of the input block and then the last bit of the input block 
            // and convert that to a byte which should be the y-coordinate.
            temporary.Set(0, bits.Get(0));
            temporary.Set(1, bits.Get(5));

            // Since our temporary BitSet only has a capacity of two, then first element of the array is 
            // the two bites converted to a byte.
            byte[] temporaryBytes = temporary.ToBytesInBigEndian();
            int y = temporaryBytes.Length > 0 ? temporaryBytes[0] : 0;

            // Calculate the x-position of the substitution.
            // We will do the same process as above but this time for all the bits of the output block.
            temporary = new BitSet(4);
            temporary.Set(0, bits.Get(1));
            temporary.Set(1, bits.Get(2));
            temporary.Set(2, bits.Get(3));
            temporary.Set(3, bits.Get(4));

            temporaryBytes = temporary.ToBytesInBigEndian();
            int x = temporaryBytes.Length > 0 ? temporaryBytes[0] : 0;

            // Execute the substitution for a s-box at x and y.
            Substitute(x, y, bits);
        }

        /// <summary>
        /// Retrieves a clone of the value at the <paramref name="x"/> and <paramref name="y"/> position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public BitSet Get(int x, int y) => (BitSet)values[y][x].Clone();

        /// <summary>
        /// Convert a set of <see cref="byte"/> arrays to a <see cref="Substitution"/>.
        /// </summary>
        /// <param name="values">The set of <see cref="byte"/> arrays.</param>
        public static implicit operator Substitution(byte[][] values) => new Substitution(BytesToBitSets(values));

        /// <summary>
        /// Convert a set of <see cref="byte"/> arrays to a set of <see cref="BitSet"/> arrays.
        /// </summary>
        /// <param name="values">The set of <see cref="byte"/> arrays.</param>
        private static BitSet[][] BytesToBitSets(byte[][] values)
        {
            BitSet[][] buffers = new BitSet[values.Length][];
            for (int i = 0; i < values.Length; i++)
            {
                BitSet[] bitSetValues = new BitSet[values[i].Length];
                for (int j = 0; j < values[i].Length; j++)
                {
                    bitSetValues[j] = new BitSet(values[i][j]);
                }

                buffers[i] = bitSetValues;
            }

            return buffers;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="Substitution"/>.
        /// </summary>
        public IEnumerator<BitSet[]> GetEnumerator() => values.AsEnumerable().GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="Substitution"/>.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
