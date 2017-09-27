using System;
using System.Collections.Generic;
using System.Timers;

namespace NetworkCryptography.App
{
    internal static class CoreApp
    {
        public static bool IsRunning => tickTimer.Enabled;
        public static Client Client { get; private set; }
        public static Server Server { get; private set; }

        private const double TickInterval = 0.0166666667;
        private static readonly Timer tickTimer;

        private static int top = Console.CursorTop;
        private static int messageInputTop;

        static CoreApp()
        {
            tickTimer = new Timer(TickInterval)
            {
                AutoReset = true
            };

            tickTimer.Elapsed += (sender, args) => Tick();
        }

        public static void RunAsClient(string ip, int port)
        {
            // If a client or server has already been initialized, we can't re-initialize!
            if (Client != null || Server != null) return;

            Client = new Client();
            Client.Connect(ip, port);

            Initialize();
        }

        public static void RunAsServer(int port)
        {
            // If a client or server has already been initialized, we can't re-initialize!
            if (Client != null || Server != null) return;

            Server = new Server(port);
            Server.Start();

            Initialize();
        }

        private static void Initialize()
        {
            tickTimer.Start();
            HandleChatInterface();
        }

        private static void Tick()
        {
            Server?.Tick();
            Client?.Tick();
        }

        private static void HandleChatInterface()
        {
            while (IsRunning)
            {
                messageInputTop = Console.WindowHeight - 1;
                if (top > messageInputTop - 1)
                {
                    messageInputTop = top + 1;
                }

                int dividerLineTop = messageInputTop - 1;
                Console.SetCursorPosition(0, dividerLineTop);
                Console.Write(new string('-', Console.WindowWidth));

                Console.SetCursorPosition(0, messageInputTop);
                string message = $"{"glum"}: {ReadLine.Read(string.Empty, string.Empty, false)}";

                Console.SetCursorPosition(0, messageInputTop);
                Console.Write(new string(' ', message.Length));

                PrintToScreen(message);

                Console.SetCursorPosition(0, dividerLineTop);
                Console.Write(message + new string(' ', Console.WindowWidth - message.Length));

                Console.WindowTop = messageInputTop - (Console.WindowHeight - 1);
            }
        }

        public static void PrintToScreen(string message)
        {
            Console.SetCursorPosition(0, top);
            Console.WriteLine(message);
            top = Console.CursorTop;

            Console.SetCursorPosition(0, messageInputTop);
        }
    }
}
