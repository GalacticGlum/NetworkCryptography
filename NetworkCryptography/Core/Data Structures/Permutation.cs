/*
 * Author: Shon Verch
 * File Name: Permutation.cs
 * Project Name: NetworkCryptography
 * Creation Date: 10/21/2017
 * Modified Date: 10/21/2017
 * Description: A data structure which stores the positional permutation values of a permutation.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NetworkCryptography.Core.DataStructures
{
    /// <summary>
    /// A data structure which stores the positional permutation values of a permutation.
    /// </summary>
    public class Permutation : IEnumerable<int>
    {
        /// <summary>
        /// The values of the <see cref="Permutation"/>.
        /// </summary>
        private readonly int[] values;

        /// <summary>
        /// Initializes a <see cref="Permutation"/> with a set of <paramref name="positions"/>.
        /// </summary>
        /// <param name="positions">The values to initialize the <see cref="Permutation"/> with.</param>
        public Permutation(int[] positions)
        {
            values = positions;
        }

        /// <summary>
        /// Permute a specified <see cref="BitSet"/> with this permutation table.
        /// </summary>
        /// <param name="bits">The <see cref="BitSet"/> to permute.</param>
        public void Permute(BitSet bits)
        {
            BitSet buffer = new BitSet(values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                buffer.Set(i, bits.Get(values[i] - 1));
            }

            bits.Replace(buffer);
        }

        /// <summary>
        /// Implicit conversion from an <see cref="Array"/> of integers to a <see cref="Permutation"/>.
        /// </summary>
        /// <param name="values">The <see cref="Array"/> of positional values.</param>
        public static implicit operator Permutation(int[] values) => new Permutation(values);

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="Permutation"/>.
        /// </summary>
        public IEnumerator<int> GetEnumerator() => values.AsEnumerable().GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="Permutation"/>.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
