using System;

namespace NetworkCryptography.Core.DataStructures
{
    public sealed class PaddedBuffer
    {
        public ulong this[int index]
        {
            get => Data.Get(index);
            set => Data.Set(index, value);
        }

        public int Count => Data.Count;

        public BlockBuffer Data { get; set; }
        public int PaddingLength { get; }

        public PaddedBuffer(string value)
        {
            PaddingLength = (int)Math.Ceiling(value.Length / (double)BlockBuffer.BlockSizeForString) * BlockBuffer.BlockSizeForString - value.Length;
            for (int i = 0; i < PaddingLength; i++)
            {
                value += Convert.ToString(PaddingLength - i);
            }

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
    }
}
