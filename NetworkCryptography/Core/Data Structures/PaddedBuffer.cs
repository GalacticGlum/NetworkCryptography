/*
 * Author: Shon Verch
 * File Name: PaddedBuffer.cs
 * Project Name: NetworkCryptography
 * Creation Date: 10/22/2017
 * Modified Date: 10/22/2017
 * Description: An extension of the BlockBuffer which pads and unpads data.
 */

using System;
using System.Data.SqlTypes;
using System.IO;

namespace NetworkCryptography.Core.DataStructures
{
    /// <summary>
    /// An extension of the <see cref="BlockBuffer"/> which pads and unpads data. 
    /// </summary>
    public sealed class PaddedBuffer
    {
        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        public ulong this[int index]
        {
            get => Data.Get(index);
            set => Data.Set(index, value);
        }

        /// <summary>
        /// The amount of blocks in the <see cref="BlockBuffer"/>.
        /// </summary>
        public int Count => Data.Count;

        /// <summary>
        /// The <see cref="BlockBuffer"/> of data.
        /// </summary>
        public BlockBuffer Data { get; set; }

        /// <summary>
        /// The amount of padding applied to the data.
        /// </summary>
        public int PaddingLength { get; }

        /// <summary>
        /// Initializes a <see cref="PaddedBuffer"/> with a specified string value performing any padding if necessary.
        /// </summary>
        /// <param name="value"></param>
        public PaddedBuffer(string value)
        {
            // Pad the value.
            PaddingLength = (int)Math.Ceiling(value.Length / (double)BlockBuffer.BlockSizeForString) * BlockBuffer.BlockSizeForString - value.Length;
            for (int i = 0; i < PaddingLength; i++)
            {
                value += Convert.ToString(PaddingLength - i);
            }

            // Convert the string data to a BlockBuffer (or more generically an array of 64-bit integers).
            Data = new BlockBuffer(value.Length / BlockBuffer.BlockSizeForString);
            for (int i = 0; i < Data.Count; i++)
            {
                ulong block = 0;
                for (int j = 0; j < BlockBuffer.BlockSizeForString; j++)
                {
                    block += Convert.ToUInt64(value[j + i * BlockBuffer.BlockSizeForString]) << (BlockBuffer.BlockSizeForString - 1 - j) * BlockBuffer.CharacterSizeInBits;
                }

                Data[i] = block;
            }
        }

        /// <summary>
        /// Initializes a <see cref="PaddedBuffer"/> with a specified array of bytes.
        /// </summary>
        /// <param name="data"></param>
        public PaddedBuffer(byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data, false))
            {
                byte[] blockData = new byte[data.Length - sizeof(int)];
                stream.Read(blockData, 0, blockData.Length);

                Data = new BlockBuffer(blockData);

                byte[] paddingLengthBytes = new byte[sizeof(int)];
                stream.Read(paddingLengthBytes, 0, paddingLengthBytes.Length);

                PaddingLength = BitConverter.ToInt32(paddingLengthBytes, 0);
            }
        }

        /// <summary>
        /// Converts the <see cref="PaddedBuffer"/> to a string value performing any unpadding if necessary.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string result = string.Empty;
            foreach (ulong block in Data)
            {
                for (int j = 0; j < BlockBuffer.BlockSizeForString; j++)
                {
                    ulong character = block >> (BlockBuffer.BlockSizeForString - 1 - j) * BlockBuffer.CharacterSizeInBits;
                    character &= 0xffff;

                    result += Convert.ToChar(character);
                }
            }

            int dataLength = Data.Count * BlockBuffer.BlockSizeForString - PaddingLength;
            return result.Substring(0, dataLength);
        }

        /// <summary>
        /// Converts the <see cref="PaddedBuffer"/> to an array of bytes.
        /// </summary>
        public byte[] ToBytes()
        {
            // We want our BlockBuffer data and also the length of the padding applied to the data so we can unpad.
            using (MemoryStream stream = new MemoryStream(Data.Count * BlockBuffer.BlockSize + sizeof(int)))
            {
                byte[] dataBytes = Data.ToBytes();
                stream.Write(dataBytes, 0, dataBytes.Length);

                byte[] paddingLengthInBytes = BitConverter.GetBytes(PaddingLength);
                stream.Write(paddingLengthInBytes, 0, paddingLengthInBytes.Length);

                return stream.ToArray();
            }
        }
    }
}
