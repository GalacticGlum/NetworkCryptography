/*
 * Author: Shon Verch
 * File Name: BlockBuffer.cs
 * Project Name: NetworkCryptography
 * Creation Date: 10/22/2017
 * Modified Date: 10/22/2017
 * Description: A data structure which stores a set of 64-bit blocks of data.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NetworkCryptography.Core.DataStructures
{
    /// <summary>
    /// A data structure which stores a set of 64-bit blocks of data.
    /// </summary>
    public sealed class BlockBuffer : ICloneable, IEnumerable<ulong>
    {
        /// <summary>
        /// The size of a block in bytes.
        /// </summary>
        public const int BlockSize = sizeof(ulong);

        /// <summary>
        /// The size of a character in bits.
        /// </summary>
        public const int CharacterSizeInBits = sizeof(char) * 8;

        /// <summary>
        /// The size of a block (in bytes) but for a string.
        /// This is different because our strings are encoded using
        /// unicode which uses 2 bytes per character.
        /// </summary>
        public const int BlockSizeForString = BlockSize / sizeof(char);

        /// <summary>
        /// The data of the <see cref="BlockBuffer"/> in a <see cref="ReadOnlyCollection{T}"/>.
        /// </summary>
        public ReadOnlyCollection<ulong> Data => new ReadOnlyCollection<ulong>(blocks);

        /// <summary>
        /// The amount of blocks in the <see cref="BlockBuffer"/>.
        /// </summary>
        public int Count => blocks.Length;

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        public ulong this[int index]
        {
            get => Get(index);
            set => Set(index, value);
        }

        /// <summary>
        /// The internal block data.
        /// </summary>
        private readonly ulong[] blocks;

        /// <summary>
        /// Initializes a <see cref="BlockBuffer"/> with a set of block data.
        /// </summary>
        /// <param name="blocks">The block data to initialize the <see cref="BlockBuffer"/> with.</param>
        public BlockBuffer(IEnumerable<ulong> blocks)
        {
            this.blocks = blocks.ToArray();
        }

        /// <summary>
        /// Initializes an empty <see cref="BlockBuffer"/> with a length.
        /// </summary>
        /// <param name="length">The length of the <see cref="BlockBuffer"/>.</param>
        public BlockBuffer(int length)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            blocks = new ulong[length];
        }

        /// <summary>
        /// Initializes a <see cref="BlockBuffer"/> with the data of another <see cref="BlockBuffer"/>.
        /// </summary>
        /// <param name="other">The other <see cref="BlockBuffer"/> to initialize with.</param>
        public BlockBuffer(BlockBuffer other)
        {
            if (other == null)
            {
                throw new NullReferenceException(nameof(other));
            }

            blocks = other.blocks;
        }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The index of the element.</param>
        /// <returns></returns>
        public ulong Get(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return blocks[index];
        }

        /// <summary>
        /// Sets the element at the specified index with a specified value.
        /// </summary>
        /// <param name="index">The index of the element.</param>
        /// <param name="value">The value to assign the element at the index.</param>
        public void Set(int index, ulong value)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            blocks[index] = value;
        }
            
        /// <summary>
        /// Copies a specified number of blocks from a source <see cref="BlockBuffer"/> to this block buffer starting at a particular offset.
        /// </summary>
        /// <param name="source">The source <see cref="BlockBuffer"/> to copy from.</param>
        /// <param name="startSourceIndex">The index to start copying from.</param>
        /// <param name="startDestinationIndex">The index to start writing to.</param>
        /// <param name="copyLength">The amount of blocks to copy.</param>
        public void BlockCopy(BlockBuffer source, int startSourceIndex, int startDestinationIndex, int copyLength)
        {
            if (source.IsOutOfRange(startSourceIndex) || IsOutOfRange(startDestinationIndex) || copyLength > Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            Buffer.BlockCopy(source.blocks, startSourceIndex * BlockSize, blocks, startDestinationIndex * BlockSize, copyLength * BlockSize);
        }

        /// <summary>
        /// Determines whether a specified index is out of range.
        /// </summary>
        /// <param name="index">Th index to check.</param>
        /// <returns>A <see cref="bool"/> indicating whether the specified index is out of range.</returns>
        private bool IsOutOfRange(int index) => index < 0 || index >= Count;

        /// <summary>
        /// Clones the <see cref="BlockBuffer"/>.
        /// </summary>
        /// <returns>The clone <see cref="BlockBuffer"/> in generic <see cref="object"/> form.</returns>
        public object Clone() => new BlockBuffer(this);

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="BlockBuffer"/>.
        /// </summary>
        public IEnumerator<ulong> GetEnumerator() => blocks.AsEnumerable().GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="BlockBuffer"/>.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
