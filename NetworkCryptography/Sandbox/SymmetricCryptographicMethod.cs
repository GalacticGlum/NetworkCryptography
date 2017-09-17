/*
 * Author: Shon Verch
 * File Name: SymmetricCryptographicMethod.cs
 * Project Name: NetworkCryptography
 * Creation Date: 9/8/2017
 * Modified Date: 9/8/2017
 * Description: Generic symmetric cryptographic implementation which allows a cient to specify the round f(k, r) function.
 */

using System;
using System.Linq;
using System.Text;
using Sandbox.Helpers;

namespace Sandbox
{
    /// <summary>
    /// Generic symmetric cryptographic method.
    /// </summary>
    public abstract class SymmetricCryptographicMethod : ICryptographicMethod
    {
        public const int BlockSize = 32;

        public string Encrypt(string message)
        {
            throw new System.NotImplementedException();
        }

        public string Decrypt(string encryptedMessage)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the blocks in BlockSize from the text string.
        /// </summary>
        /// <param name="text">The string to convert to blocks.</param>
        /// <returns>An array of unsigned integers containing the blocks.</returns>
        private uint[] GetBlocks(string text)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(text);

            int blockCount = (int) Math.Ceiling(buffer.Length / (double) BlockSize);
            uint[] blocks = new uint[blockCount];

            // TODO: Currently this operation is O(n * 2), this should probably be reduced into an unsafe operation
            // which is O(n)
            for (int i = 0; i < blockCount; i++)
            {
                byte[] block = buffer.Skip(BlockSize * i).Take(BlockSize).ToArray();
                blocks[i] = block.ToUint32();
            }

            return blocks;
        }
    }
}
