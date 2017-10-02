/*
 * Author: Shon Verch
 * File Name: EnumHelper.cs
 * Project: NetworkCryptography
 * Creation Date: 9/25/2017
 * Modified Date: 9/25/2017
 * Description: Collection of useful enum functionality. 
 */

using System;

namespace NetworkCryptography.Core.Helpers
{
    /// <summary>
    /// Collection of useful enum functionality.
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// Retrieves a specific value of an enum by an <value>index</value>.
        /// </summary>
        /// <typeparam name="T">The type of the enum.</typeparam>
        /// <param name="index">The index of the value to retrieve.</param>
        /// <returns>The value at the specified <value>index</value>.</returns>
        public static T GetValue<T>(int index)
        {
            // Check whether the passed in type is an enum, we do this because we cannot constraint generics to enums.
            // If the passed in type is NOT an enum then throw an ArgumentException.
            Type enumType = typeof(T);
            if (!enumType.IsEnum)
            {
                throw new ArgumentException($"{enumType.FullName} is not an enum!");
            }

            // Get all the possible values in the enum.
            T[] values = (T[])Enum.GetValues(typeof(T));
            return values[index];
        }
    }
}
