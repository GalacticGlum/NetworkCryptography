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

                    Console.Clear();
                    CoreApp.RunAsClient(ip, int.Parse(port));

                    break;
                case 1:
                    Console.Clear();

                    port = ConsoleDisplay.InputField("Please enter the port");
                    Console.Clear();

                    CryptographyMethodType selectedMethod = EnumHelper.GetValue<CryptographyMethodType>(ConsoleDisplay.Menu("What cryptography method would you like to use?", cryptographyTypeNames));

                    Console.Clear();

                    // Print server stats
                    Console.WriteLine($"Server started{Environment.NewLine}");
                    Console.WriteLine($"IP Address: {NetworkHelper.GetLocalIpAddress()}");
                    Console.WriteLine($"Port: {port}");
                    Console.WriteLine($"Cryptography method: {GetCryptographyMethodName(selectedMethod)}");

                    Console.WriteLine(StringHelper.Underscore.Multiply(25));

                    CoreApp.RunAsServer(int.Parse(port));

                    break;
            }

            Console.ReadKey();
        }

        private static string GetCryptographyMethodName(CryptographyMethodType type) => cryptographyTypeNames[(int)type];
    }
}
