/*
 * Author: Shon Verch
 * File Name: ArrayHelper.cs
 * Project Name: NetworkCryptography
 * Creation Date: 9/18/2017
 * Modified Date: 9/19/2017
 * Description: A collection of extra array functionality.
 */

using System;
using System.Runtime.InteropServices;

namespace NetworkCryptography.Core.Helpers
{
    /// <summary>
    /// A collection of extra array functionality.
    /// </summary>
    public static class ArrayHelper
    {
        /// <summary>
        /// Combines two arrays.
        /// </summary>
        /// <typeparam name="T">The type of the arrays.</typeparam>
        /// <param name="first">The first array.</param>
        /// <param name="second">The second array.</param>
        /// <returns>A new array containing the values of both arrays.</returns>
        public static T[] Combine<T>(T[] first, T[] second)
        {
            T[] result = new T[first.Length + second.Length];

            Buffer.BlockCopy(first, 0, result, 0, first.Length);
            Buffer.BlockCopy(second, 0, result, first.Length, second.Length);

            return result;
        }

        /// <summary>
        /// Prepends a value to a <see cref="Array"/>.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="prependValue">The value to prepend.</param>
        /// <param name="array">The array to prepend the value to.</param>
        /// <returns>A new <see cref="Array"/> containing the values of both.</returns>
        public static T[] PrependValue<T>(this T[] array, T prependValue)
        {
            int size = Marshal.SizeOf(typeof(T));

            T[] result = new T[1 + array.Length];

            result[0] = prependValue;
            Buffer.BlockCopy(array, 0, result, size, array.Length * size);

            return result;
        }

        /// <summary>
        /// Appends a value to a <see cref="Array"/>.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="array">The array to append the value.</param>
        /// <param name="appendValue">The value to append to the <see cref="Array"/>.</param>
        /// <returns></returns>
        public static T[] AppendValue<T>(this T[] array, T appendValue)
        {
            int size = Marshal.SizeOf(typeof(T));

            T[] result = new T[array.Length + 1];

            Buffer.BlockCopy(array, 0, result, 0, array.Length * size);
            result[result.Length - 1] = appendValue;

            return result;
        }

        /// <summary>
        /// Creates a new array of <value>length</value> and initializes it with a value. 
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="value">The value to fill the array with.</param>
        /// <param name="length">The length of the array</param>
        /// <returns></returns>
        public static T[] MakeFilled<T>(T value, int length)
        {
            T[] array = new T[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = value;
            }

            return array;
        }

        /// <summary>
        /// Pads an array with a specified value for a specified length.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">The array to pad.</param>
        /// <param name="paddingLength">The length of the padding.</param>
        /// <param name="padValue">The value to pad the array.</param>
        /// <param name="suffix">Indicates whether the padding should be suffixed or prefixed. 
        /// By default this value is false; therefore, we prefix the padding.</param>
        /// <returns>A new padded array.</returns>
        public static T[] Pad<T>(T[] array, int paddingLength, T padValue, bool suffix = false)
        {
            T[] padding = MakeFilled(padValue, paddingLength);
            T[] result = suffix ? Combine(array, padding) : Combine(padding, array);

            return result;
        }

        /// <summary>
        /// Iterates through an array and prints each value.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="array">The array to iterate through.</param>
        /// <param name="elementSeperator">THe seperator between array elements.</param>
        public static void Print<T>(this T[] array, string elementSeperator = "")
        {
            foreach (T value in array)
            {
                Console.Write(value + elementSeperator);
            }

            Console.WriteLine();
        }
    }
}
