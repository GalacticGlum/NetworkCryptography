using System;
using System.Timers;
using NetworkCryptography.Core;
using NetworkCryptography.Core.Helpers;
using NetworkCryptography.Core.Logging;

namespace NetworkCryptography.Server
{
    internal static class CoreApp
    {
        public static bool IsRunning => tickLoop.Running;
        public static Server Server { get; private set; }

        private static readonly TickLoop tickLoop;
        private static readonly string[] cryptographyTypeNames =
        {
            "Caesar Cipher",
            "Data Encryption Standard (DES)",
            "Rivest–Shamir–Adleman (RSA) Scheme"
        };

        static CoreApp()
        {
            tickLoop = new TickLoop(Tick);
        }

        public static void Run(int port, CryptographyMethodType cryptographyMethodType)
        {
            Server = new Server(port);
            Server.Start();

            Initialize();
            Logger.Destination = LoggerDestination.All;
            Logger.Log($"Started server on <{NetworkHelper.GetLocalIpAddress()}:{port}>", LoggerVerbosity.Plain);
        }

        private static void Initialize()
        {
            tickLoop.Start();
        }

        private static void Tick(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Server?.Tick();
        }

        private static void ProcessConsoleCommands()
        {
            while (IsRunning)
            {
                Console.Write(">>> ");
                string line = Console.ReadLine();
                if (string.IsNullOrEmpty(line)) continue;

                switch (line)
                {
                    case "quit":
                        Exit();
                        break;
                    default:
                        Console.WriteLine($"'{line}' is not recognized as a command.{Environment.NewLine}");
                        break;
                }
            }
        }

        private static void Exit()
        {
            tickLoop.Stop();
            Logger.FlushMessageBuffer();
        }

        private static void Main(string[] args)
        {
            int port = int.Parse(ConsoleDisplay.InputField("Please enter the port you would like to run the server on."));
            Console.Clear();

            CryptographyMethodType selectedMethod = EnumHelper.GetValue<CryptographyMethodType>(ConsoleDisplay.Menu(
                "What cryptography method would you like to use?", cryptographyTypeNames));

            Console.Clear();
            Run(port, selectedMethod);
            ProcessConsoleCommands();
        }
    }
}
