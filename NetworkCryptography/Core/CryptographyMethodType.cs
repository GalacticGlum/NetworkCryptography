/*
 * Author: Shon Verch
 * File Name: CryptographyMethodType.cs
 * Project: NetworkCryptography
 * Creation Date: 9/25/2017
 * Modified Date: 9/25/2017
 * Description: Different types of cryptography methods available.
 */

namespace NetworkCryptography.Core
{
    /// <summary>
    /// Different types of cryptography methods available.
    /// </summary>
    public enum CryptographyMethodType
    {
        /// <summary>
        /// Caesar Cipher
        /// </summary>
        Caesar,

        /// <summary>
        /// Data Encryption Standard
        /// </summary>
        DES,
        
        /// <summary>
        /// Rivest–Shamir–Adleman
        /// </summary>
        RES
    }
}
