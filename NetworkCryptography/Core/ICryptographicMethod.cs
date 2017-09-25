/*
 * Author: Shon Verch
 * File Name: ICryptographicMethod.cs
 * Project Name: NetworkCryptography
 * Creation Date: 9/7/2017
 * Modified Date: 9/7/2017
 * Description: A generic interface for all cryptography method.
 */

namespace NetworkCryptography.Core
{
    /// <summary>
    /// Interface for all cryptography methods.
    /// </summary>
    public interface ICryptographicMethod
    {
        /// <summary>
        /// Encrypt the message string.
        /// </summary>
        /// <param name="message">The plain text to encrypt.</param>
        /// <returns>The encrypted message.</returns>
        string Encrypt(string message);

        /// <summary>
        /// Decrypt the encrypted message string.
        /// </summary>
        /// <param name="encryptedMessage">The chiper text to decrypt.</param>
        /// <returns>The decrypted message.</returns>
        string Decrypt(string encryptedMessage);
    }
}
