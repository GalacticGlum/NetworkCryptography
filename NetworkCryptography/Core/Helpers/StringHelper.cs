/*
 * Author: Shon Verch
 * File Name: StringHelper.cs
 * Project: NetworkCryptography
 * Creation Date: 9/20/2017
 * Modified Date: 9/28/2017
 * Description: Collection of useful string-related functionality.
 */

using System.Linq;

namespace NetworkCryptography.Core.Helpers
{
    /// <summary>
    /// Collection of useful string-related functionality.
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Space character constant.
        /// </summary>
        public const string Space = " ";

        /// <summary>
        /// Overline character constant. 
        /// Note: This is unicode, therefore it will not print in the console.s
        /// </summary>
        public const string Overline = "‾";

        /// <summary>
        /// Underscore character constant.
        /// </summary>
        public const string Underscore = "_";

        /// <summary>
        /// Multiplies a string by a given amount.
        /// </summary>
        /// <param name="a">The string to multiply.</param>
        /// <param name="length">The amount to multiply the string by.</param>
        /// <returns>The result of the string multplication.</returns>
        public static string Multiply(this string a, int length) => string.Concat(Enumerable.Repeat(a, length));

        /// <summary>
        /// Multiplies a character by a given amount.
        /// </summary>
        /// <param name="a">The character to multiply.</param>
        /// <param name="length">The amount to multiply the character by.</param>
        /// <returns>The result of the string multplication.</returns>
        public static string Multiply(this char a, int length) => new string(a, length);
    }
}
