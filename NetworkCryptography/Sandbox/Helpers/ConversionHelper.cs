/*
 * Author: Shon Verch
 * File Name: ConversionHelper.cs
 * Project Name: NetworkCryptography
 * Creation Date: 9/8/2017
 * Modified Date: 9/8/2017
 * Description: A collection of extension methods which simplifies conversion operations.
 */

using System;

namespace Sandbox.Helpers
{
    /// <summary>
    /// Conversion extension methods.
    /// </summary>
    public static class ConversionHelper
    {
        /// <summary>
        /// Converts an array of bytes to an unsigned integer.
        /// </summary>
        /// <param name="buffer">The bytes to convert.</param>
        /// <returns>The converted unsigned integer.</returns>
        public static uint ToUint32(this byte[] buffer)
        {
            uint result = 0;
            for (int i = 0; i < buffer.Length; i++)
            {
                result += (uint)Math.Pow(256, buffer.Length - 1 - i) * buffer[i];
            }

            return result;
        }
    }
}
