/*
 * Author: Shon Verch
 * File Name: RsaKeySet.cs
 * Project Name: NetworkCryptography
 * Creation Date: 10/22/2017
 * Modified Date: 10/22/2017
 * Description: A set of keys used for RSA cryptography.
 */

using System.Numerics;

namespace NetworkCryptography.Core.DataStructures
{
    /// <summary>
    /// A set of keys used for RSA cryptography.
    /// </summary>
    public struct RsaKeySet
    {
        /// <summary>
        /// Public key used for encryption.
        /// </summary>
        public BigInteger PublicExponent { get; set; }

        /// <summary>
        /// Private key used for decryption.
        /// </summary>
        public BigInteger PrivateExponent { get; set; }

        /// <summary>
        /// Used for both encryption and decryption.
        /// </summary>
        public BigInteger Modulus { get; set; }

        /// <summary>
        /// Initializes a new <see cref="RsaKeySet"/>.
        /// </summary>
        /// <param name="publicExponent">The public key used for encryption.</param>
        /// <param name="privateExponent">The private key used for decryption.</param>
        /// <param name="modulus">The key used for both encryption and decryption.</param>
        public RsaKeySet(BigInteger publicExponent, BigInteger privateExponent, BigInteger modulus)
        {
            PublicExponent = publicExponent;
            PrivateExponent = privateExponent;
            Modulus = modulus;
        }
    }
}
