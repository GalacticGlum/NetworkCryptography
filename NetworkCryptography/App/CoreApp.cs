using System;
using System.Timers;
using NetworkCryptography.Core.Helpers;

namespace NetworkCryptography.App
{
    internal static class CoreApp<T> where T : IAppProvider, new()
    {
        private const double TickInterval = 0.0166666667;

        private static T appProvider;
        private static readonly Timer tickTimer;

        static CoreApp()
        {
            tickTimer = new Timer(TickInterval);
            tickTimer.Elapsed += (sender, args) => appProvider.Tick();
        }

        private static void Run()
        {
            appProvider = new T();
            appProvider.Initialize();

            tickTimer.Start();
        }
    }

    internal static class ConsoleApp
    {
        private static void Main(string[] args)
        {
            int selection = ConsoleDisplay.Menu("Welcome to a generic live chat application", "Start Client", "Start Server");
            switch (selection)
            {
                case 0:
                    Console.WriteLine("Starting client...");
                    break;
                case 1:
                    Console.WriteLine("Starting server...");
                    break;
            }

            Console.ReadKey();
        }
    }
}
