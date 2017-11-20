/*
 * Author: Shon Verch
 * File Name: CryptographyHelper.cs
 * Project Name: NetworkCryptography
 * Creation Date: 10/20/2017
 * Modified Date: 10/22/2017
 * Description: A collection of useful cryptography-related functionality.
 */

using System;
using System.Security.Cryptography;
using NetworkCryptography.Core.DataStructures;

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
                // The keys are hardcoded as this is only to demonstrate the cryptography applications and not
                // for actual security.
                case CryptographyMethodType.Caesar:
                    return new CaeserCryptographicMethod();
                case CryptographyMethodType.DES:
                    return new DesCryptographicMethod(89);
                case CryptographyMethodType.RES:
                    return new RsaCryptographicMethod(new RsaKeySet(3079, 487, 5767));
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        /// <summary>
        /// Generate a random block of bytes with a specified length.
        /// </summary>
        /// <param name="length">The length of the block to generate.</param>
        /// <returns></returns>
        public static ulong GenerateRandomBlock(int length)
        {
            byte[] password = new byte[length];
            using (RNGCryptoServiceProvider randomCryptoService = new RNGCryptoServiceProvider())
            {
                randomCryptoService.GetBytes(password);
                return BitConverter.ToUInt64(password, 0);
            }
        }
    }
}
