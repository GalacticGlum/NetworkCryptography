/*
 * Author: Shon Verch
 * File Name: RsaCryptographyMethod.cs
 * Project Name: NetworkCryptography
 * Creation Date: 10/22/2017
 * Modified Date: 10/22/2017
 * Description: RSA scheme cryptography implementation.
 */

using System;
using System.IO;
using System.Numerics;
using NetworkCryptography.Core.DataStructures;

namespace NetworkCryptography.Core
{
    /// <summary>
    /// RSA scheme cryptography implementation.
    /// </summary>
    public class RsaCryptographicMethod : ICryptographicMethod
    {
        /*
         * This implementation of RSA is not very secure - quite so.
         * Due to variable size limitations (as the ciphertext is much to large for a datatype)
         * we encrypt and decrypt every character separately. This should not be done in a 
         * real-world application but is fine for a proof of concept. In addition, all clients
         * share the smae keys which again should not be done practically but is fine for showing
         * the implementation of RSA.
         * 
         * It is certainly possible to expand this and encrypt blocks instead of a per-character scheme.
         * Though it will still exposes redundancies in the ciphertext which could be used to break
         * the cipher.
         */

        /// <summary>
        /// The keys to use for encryption and decryption.
        /// </summary>
        private readonly RsaKeySet keys;

        /// <summary>
        /// Initializes the <see cref="RsaCryptographicMethod"/> with a <see cref="RsaKeySet"/>.
        /// </summary>
        /// <param name="keys">The keys to initialize with.</param>
        public RsaCryptographicMethod(RsaKeySet keys)
        {
            this.keys = keys;
        }

        /// <summary>
        /// Encrypt plaintext.
        /// </summary>
        /// <param name="plaintext">The plaintext to encrypt.</param>
        /// <returns>An array of bytes containing the ciphertext.</returns>
        public byte[] Encrypt(string plaintext)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                foreach (char character in plaintext)
                {
                    BigInteger result = BigInteger.ModPow((int)character, keys.PublicExponent, keys.Modulus);
                    byte[] bytes = BitConverter.GetBytes((int)result);
                    stream.Write(bytes, 0, bytes.Length);
                }

                return stream.ToArray();
            }
        }

        /// <summary>
        /// Decrypt ciphertext.
        /// </summary>
        /// <param name="ciphertext">The ciphertext to decrypt.</param>
        /// <returns>A string value representing the plaintext.</returns>
        public string Decrypt(byte[] ciphertext)
        {
            using (MemoryStream stream = new MemoryStream(ciphertext, false))
            {
                string plaintext = string.Empty;
                for (int i = 0; i < ciphertext.Length / sizeof(int); i++)
                {
                    byte[] buffer = new byte[sizeof(int)];
                    stream.Read(buffer, 0, buffer.Length);

                    int value = BitConverter.ToInt32(buffer, 0);
                    BigInteger result = BigInteger.ModPow(value, keys.PrivateExponent, keys.Modulus);

                    plaintext += Convert.ToChar((int)result);
                }

                return plaintext;
            }
        }
    }
}
