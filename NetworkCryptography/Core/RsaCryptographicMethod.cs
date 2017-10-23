/*
 * Author: Shon Verch
 * File Name: RsaCryptographyMethod.cs
 * Project Name: NetworkCryptography
 * Creation Date: 10/22/2017
 * Modified Date: 10/22/2017
 * Description: RSA scheme cryptography implementation.
 */

using System.Numerics;
using NetworkCryptography.Core.DataStructures;

namespace NetworkCryptography.Core
{
    /// <summary>
    /// RSA scheme cryptography implementation.
    /// </summary>
    public class RsaCryptographicMethod 
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
        /// <returns>An array of integers containing the ciphertext.</returns>
        public int[] Encrypt(string plaintext)
        {
            int[] ciphertext = new int[plaintext.Length];
            for (int i = 0; i < ciphertext.Length; i++)
            {
                BigInteger result = BigInteger.ModPow((int)plaintext[i], keys.PublicExponent, keys.Modulus);
                ciphertext[i] = (int) result;
            }

            return ciphertext;
        }

        /// <summary>
        /// Decrypt ciphertext.
        /// </summary>
        /// <param name="ciphertext">The ciphertext to decrypt.</param>
        /// <returns>A string value representing the plaintext.</returns>
        public string Decrypt(int[] ciphertext)
        {
            string plaintext = string.Empty;
            foreach (int value in ciphertext)
            {
                BigInteger result = BigInteger.ModPow(value, keys.PrivateExponent, keys.Modulus);
                plaintext += (char) result;
            }

            return plaintext;
        }
    }
}
