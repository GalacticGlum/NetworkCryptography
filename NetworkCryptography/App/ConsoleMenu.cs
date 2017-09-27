using System;
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
            int top = Console.CursorTop;

            while (isRunning)
            {
                int messageInputTop = Console.WindowHeight - 1;
                if (top > messageInputTop - 1)
                {
                    messageInputTop = top + 1;
                }

                int dividerLineTop = messageInputTop - 1;
                Console.SetCursorPosition(0, dividerLineTop);
                Console.Write(new string('-', Console.WindowWidth));

                Console.SetCursorPosition(0, messageInputTop);
                string message = $"{username}: {ReadLine.Read(string.Empty, string.Empty, false)}";

                Console.SetCursorPosition(0, messageInputTop);
                Console.Write(new string(' ', message.Length));

                Console.SetCursorPosition(0, top);
                Console.WriteLine(message);
                top = Console.CursorTop;

                Console.SetCursorPosition(0, dividerLineTop);
                Console.Write(message + new string(' ', Console.WindowWidth - message.Length));
                
                Console.WindowTop = messageInputTop - (Console.WindowHeight - 1);
            }
        }

        private static string GetCryptographyMethodName(CryptographyMethodType type) => cryptographyTypeNames[(int)type];
    }
}
