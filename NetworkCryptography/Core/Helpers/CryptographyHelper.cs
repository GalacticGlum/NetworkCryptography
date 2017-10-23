/*
 * Author: Shon Verch
 * File Name: CryptographyHelper.cs
 * Project Name: NetworkCryptography
 * Creation Date: 10/20/2017
 * Modified Date: 10/22/2017
 * Description: A collection of useful cryptography-related functionality.
 */

using System;
using System.Numerics;
using System.Security.Cryptography;

namespace NetworkCryptography.Core.Helpers
{
    /// <summary>
    /// A collection of useful cryptography-related functionality.
    /// </summary>
    public static class CryptographyHelper
    {
        /// <summary>
        /// Random service provider.
        /// </summary>
        private static readonly Random random = new Random();

        /// <summary>
        /// Creates a <see cref="ICryptographicMethod"/> from a <see cref="CryptographyMethodType"/>.
        /// </summary>
        /// <param name="type">The type of cryptography method to create.</param>
        public static ICryptographicMethod CreateEngine(CryptographyMethodType type)
        {
            switch (type)
            {
                case CryptographyMethodType.Caesar:
                    return new CaeserCryptographicMethod();
                case CryptographyMethodType.DES:
                    return null;
                case CryptographyMethodType.RES:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        /// <summary>
        /// Generate a block of bytes.
        /// </summary>
        /// <returns></returns>
        public static ulong GenerateRandomBlock(int bytes)
        {
            byte[] password = new byte[bytes];
            using (RNGCryptoServiceProvider randomCryptoService = new RNGCryptoServiceProvider())
            {
                randomCryptoService.GetBytes(password);
                return BitConverter.ToUInt64(password, 0);
            }
        }
    }
}
