using System;
using System.IO;
using System.Linq;
using System.Text;
using NetworkCryptography.Core.Helpers;

namespace NetworkCryptography.Server
{
    public class CBCDes
    {
        private static Random rng = new Random();

        public static string Encrypt(string plaintext, ulong key)
        {
            byte[] data = Encoding.Unicode.GetBytes(plaintext);
            data = new byte[8].Concat(data).ToArray();
            if (data.Length % 8 != 0)
            {
                int newLength = data.Length + (8 - data.Length % 8);
                data = Pad(data, newLength);
            }

            byte[] output = new byte[data.Length];
            using (MemoryStream stream = new MemoryStream(output))
            {
                ulong currentBlock = CreateInitializationVector();
                for (int i = 0; i < data.Length; i += 8)
                {
                    ulong block = BitConverter.ToUInt64(data.Skip(i).Take(8).ToArray(), 0);
                    block ^= currentBlock;

                    currentBlock = DES.Encode(block, key);
                    Console.WriteLine(currentBlock);

                    stream.Write(BitConverter.GetBytes(currentBlock), 0, 8);
                }

                byte[] bytes = stream.ToArray();
                return Encoding.Unicode.GetString(bytes);
            }
        }

        public static string Decrypt(string ciphertext, ulong key)
        {
            Console.WriteLine("##########################");

            byte[] data = Encoding.Unicode.GetBytes(ciphertext);

            byte[] output = new byte[data.Length - 8];
            using (MemoryStream stream = new MemoryStream(output))
            {
                ulong currentBlock = BitConverter.ToUInt64(data.Take(8).ToArray(), 0);
                for (int i = 8; i < data.Length; i += 8)
                {
                    ulong block = BitConverter.ToUInt64(data.Skip(i).Take(8).ToArray(), 0);
                    ulong decoded = DES.Decode(block, key);

                    ulong plaintextBuffer = currentBlock ^ decoded;
                    currentBlock = block;
                    Console.WriteLine(plaintextBuffer);

                    byte[] bytes = BitConverter.GetBytes(plaintextBuffer);
                    //Unpad(ref bytes, 8);
                    stream.Write(bytes, 0, 8);
                }

                return Encoding.Unicode.GetString(stream.ToArray());
            }
        }

        private static ulong CreateInitializationVector()
        {           
            byte[] buffer = new byte[64 / 8];
            rng.NextBytes(buffer);

            return BitConverter.ToUInt64(buffer, 0);

        }

        /// <summary>
        /// Pad data to a certain length.
        /// </summary>
        /// <param name="data">The byte array to pad.</param>
        /// <param name="targetSize">The target size of the whole buffer.</param>
        private static byte[] Pad(byte[] data, int targetSize)
        {
            byte[] buffer = new byte[targetSize];
            Array.Copy(data, 0, buffer, 0, data.Length);

            // The pad value is the delta size of our CURRENT data and our TARGET data.
            byte padValue = (byte)(targetSize - data.Length);
            for (int i = data.Length; i < targetSize; i++)
            {
                buffer[i] = padValue;
            }

            return buffer;
        }

        /// <summary>
        /// Unpad data to a certain length.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="targetSize"></param>
        /// <returns></returns>
        private static int Unpad(ref byte[] buffer, int targetSize)
        {
            // Since we pad data with the delta size between the current and target data.
            // We can check if data is padded by seeing if the last value is less than
            // our target size, and if it's greater than 0 (since we can't have negative size).
            // Otherwise, we don't have padded data, return our target size.
            if (buffer[targetSize - 1] >= targetSize || buffer[targetSize - 1] <= 0) return targetSize;

            int count = 0;
            byte padValue = buffer[targetSize - 1];

            for (int i = targetSize - 1; i > 0; i--)
            {
                // If the element at i is our padValue then increment the count.
                if (buffer[i] == padValue)
                {
                    count++;
                }
            }

            // If our count is equal to our pad value then this block of data is padded.
            // This is because our pad value is the delta size between our current and target sizes.
            // Therefore, if our count is equal to the amount of data that was padded, let's 
            // unpad our data. 
            // Otherwise, we don't have padded data, so let's return our target size.
            if (count != padValue) return targetSize;

            byte[] newData = new byte[targetSize - padValue];
            Array.Copy(buffer, 0, newData, 0, newData.Length);
            buffer = newData;

            return newData.Length;
        }
    }
}
