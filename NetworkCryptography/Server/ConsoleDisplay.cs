/*
 * Author: Shon Verch
 * File Name: ConsoleDisplay.cs
 * Project: NetworkCryptography
 * Creation Date: 9/23/2017
 * Modified Date: 10/18/2017
 * Description: Collection of useful console interface funtionality.
 */

using System;
using System.Text;
using NetworkCryptography.Core.Helpers;

namespace NetworkCryptography.Server
{
    /// <summary>
    /// Collection of useful console interface funtionality.
    /// </summary>
    public static class ConsoleDisplay
    {
        static ConsoleDisplay()
        {
            // Setup Console globals
            Console.OutputEncoding = Encoding.Unicode;
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        /// <summary>
        /// Display a menu with a specified options and dialog prompt.
        /// </summary>
        /// <param name="dialog">The dialog prompt to display above the options.</param>
        /// <param name="options">The options to display.</param>
        /// <returns>The selected option index.</returns>
        public static int Menu(string dialog, params string[] options)
        {
            bool hasSelected = false;
            int selectedIndex = 0;

            Console.CursorVisible = false;
            Console.WriteLine($"{dialog}{Environment.NewLine}");

            int topOffset = Console.CursorTop;
            int bottomOffset = 0;

            int longestOptionLength = 0;

            while (!hasSelected)
            {
                for (int i = 0; i < options.Length; i++)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;

                    if (i == selectedIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }

                    string option = $"{i + 1}. {options[i]}";
                    if (option.Length > longestOptionLength)
                    {
                        longestOptionLength = option.Length;
                    }

                    Console.WriteLine(option);
                    Console.ResetColor();
                }

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(StringHelper.Overline.Multiply(longestOptionLength));

                bottomOffset = Console.CursorTop;
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (selectedIndex > 0)
                        {
                            selectedIndex--;
                        }
                        else
                        {
                            selectedIndex = options.Length - 1;
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        if (selectedIndex < options.Length - 1)
                        {
                            selectedIndex++;
                        }
                        else
                        {
                            selectedIndex = 0;
                        }
                        break;
                    case ConsoleKey.Enter:
                        hasSelected = true;
                        break;
                }

                Console.SetCursorPosition(0, topOffset);
            }

            Console.SetCursorPosition(0, bottomOffset);
            Console.CursorVisible = true;

            return selectedIndex;
        }
        
        /// <summary>
        /// Dispaly an input field with a prompt.
        /// </summary>
        /// <param name="prompt">The prompt to display.</param>
        /// <returns>The input.</returns>
        public static string InputField(string prompt)
        {
            Console.WriteLine(prompt);
            return Console.ReadLine();
        }
    }
}
