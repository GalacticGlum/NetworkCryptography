using System;
using System.Collections.Generic;
using NetworkCryptography.Core.Helpers;

namespace NetworkCryptography.App
{
    internal static class ConsoleMenu
    {
        private static readonly string[] cryptographyTypeNames =
        {
            "Caesar Cipher",
            "Data Encryption Standard (DES)",
            "Rivest–Shamir–Adleman (RSA) Scheme"
        };

        private static void Main(string[] args)
        {
            string username = ConsoleDisplay.InputField("Please enter your user name");
            Console.Clear();

            int selection = ConsoleDisplay.Menu($"Welcome {username}!", "Start Client", "Start Server");
            switch (selection)
            {
                case 0:
                    Console.Clear();
                    string ip = ConsoleDisplay.InputField("Please enter the IP address of the server you would like to connect to");
                    string port = ConsoleDisplay.InputField($"{Environment.NewLine}Please enter the port");

                    CoreApp.RunAsClient(ip, int.Parse(port));
                    Console.Clear();

                    break;
                case 1:
                    Console.Clear();

                    port = ConsoleDisplay.InputField("Please enter the port");
                    Console.Clear();

                    CryptographyMethodType selectedMethod = EnumHelper.GetValue<CryptographyMethodType>(ConsoleDisplay.Menu("What cryptography method would you like to use?", cryptographyTypeNames));

                    Console.Clear();

                    CoreApp.RunAsServer(int.Parse(port));

                    // Print server stats
                    Console.WriteLine($"Server started{Environment.NewLine}");
                    Console.WriteLine($"IP Address: {NetworkHelper.GetLocalIpAddress()}");
                    Console.WriteLine($"Port: {port}");
                    Console.WriteLine($"Cryptography method: {GetCryptographyMethodName(selectedMethod)}");

                    Console.WriteLine(StringHelper.Underscore.Multiply(25));
                    break;
            }

            bool isRunning = true;
            string message = string.Empty;
            int top = Console.CursorTop;

            while (isRunning)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                int clearLength = 0;

                if (key.Key == ConsoleKey.Enter)
                {
                    clearLength = message.Length;

                    Console.SetCursorPosition(0, top);

                    Console.WriteLine($"{username}: {message}");
                    top = Console.CursorTop;

                    isRunning = message != "/quit";
                    message = string.Empty;
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (message.Length > 0)
                    {
                        message = message.Substring(0, message.Length - 1);
                    }
                }
                else
                {
                    message += key.KeyChar;
                }

                int messageInputTop = Console.WindowHeight;
                if (top > Console.WindowHeight)
                {
                    messageInputTop += top - Console.WindowHeight;
                }

                Console.SetCursorPosition(0, messageInputTop);
                if (clearLength > 0)
                {
                    Console.Write(new string(' ', clearLength));
                }

                Console.Write(message);
            }
        }

        private static string GetCryptographyMethodName(CryptographyMethodType type) => cryptographyTypeNames[(int)type];
    }
}
