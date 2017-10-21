/*
 * Author: Shon Verch
 * File Name: CaeserCryptographicMethod.cs
 * Project Name: NetworkCryptography
 * Creation Date: 9/7/2017
 * Modified Date: 9/8/2017
 * Description: An implementation of the Caeser Chiper cryptography method.
 */

namespace NetworkCryptography.Core
{
    /// <summary>
    /// The Caeser Chiper crytography method.
    /// </summary>
    public class CaeserCryptographicMethod : ICryptographicMethod
    {
        /// <summary>
        /// The amount to shift by (also known as the key).
        /// </summary>
        private const int ShiftOffset = 3;

        /// <summary>
        /// Shifts the message by an offset.
        /// </summary>
        /// <param name="message">The message to shift.</param>
        /// <param name="offset">The shift offset.</param>
        /// <returns>The message shifted by offset.</returns>
        private static string Shift(string message, int offset)
        {
            // Create a new array of characters which is the size of our original message.
            char[] result = new char[message.Length];
            for (int i = 0; i < result.Length; i++)
            {
                // Shift the character at in in the message by the offset and insert it into the array at i.
                result[i] = (char)(message[i] + offset);
            }

            // Convert the result char array to a string object.
            return new string(result);
        }

        /// <summary>
        /// Encrypt the message using Caeser Cipher.
        /// </summary>
        /// <param name="message">The plain text message to encrypt.</param>
        /// <returns>The encrypted message.</returns>
        public string Encrypt(string message) => Shift(message, ShiftOffset);

        /// <summary>
        /// Decrypt the encrypted message string using Caeser Cipher.
        /// </summary>
        /// <param name="encryptedMessage">The cipher text to decrypt.</param>
        /// <returns>The decrypted plain text.</returns>
        public string Decrypt(string encryptedMessage) => Shift(encryptedMessage, -ShiftOffset);
    }
}
