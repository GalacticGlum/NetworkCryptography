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
        public const int BlockSize = sizeof(ulong);
        public const int CharacterSizeInBits = sizeof(char) * 8;
        public const int BlockSizeForString = BlockSize / sizeof(char);

        public ReadOnlyCollection<ulong> Data => new ReadOnlyCollection<ulong>(blocks);
        public int Count => blocks.Length;

        public ulong this[int index]
        {
            get => Get(index);
            set => Set(index, value);
        }

        private readonly ulong[] blocks;

        public BlockBuffer(IEnumerable<ulong> value)
        {
            blocks = value.ToArray();
        }

        public BlockBuffer(int length)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            blocks = new ulong[length];
        }

        public BlockBuffer(BlockBuffer other)
        {
            if (other == null)
            {
                throw new NullReferenceException(nameof(other));
            }

            blocks = other.blocks;
        }

        public ulong Get(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return blocks[index];
        }

        public void Set(int index, ulong value)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            blocks[index] = value;
        }
            
        public void BlockCopy(BlockBuffer source, int startSourceIndex, int startDestinationIndex, int copyLength)
        {
            if (source.IsOutOfRange(startSourceIndex) || IsOutOfRange(startDestinationIndex) || copyLength > Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            Buffer.BlockCopy(source.blocks, startSourceIndex * BlockSize, blocks, 
                startDestinationIndex * BlockSize, copyLength * BlockSize);
        }

        private bool IsOutOfRange(int index) => index < 0 || index >= Count;

        public object Clone() => new BlockBuffer(this);

        public IEnumerator<ulong> GetEnumerator() => blocks.AsEnumerable().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
