using System;
using NetworkCryptography.Core.Helpers;

namespace NetworkCryptography.Server
{
    public static class ConsoleDisplay
    {
        public static int Menu(string dialog, params string[] options)
        {
            bool hasSelected = false;
            int selectedIndex = 0;

            Console.CursorVisible = false;
            Console.WriteLine($"{dialog}{Environment.NewLine}");

            int topOffset = Console.CursorTop;
            int bottomOffset = 0;

            while (!hasSelected)
            {
                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selectedIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }

                    Console.WriteLine($"{i + 1}. {options[i]}");
                    Console.ResetColor();
                }

                Console.WriteLine("_".Multiply(25));

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

        public static string InputField(string prompt)
        {
            Console.WriteLine(prompt);
            return Console.ReadLine();
        }
    }
}
