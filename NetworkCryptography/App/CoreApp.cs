using System;
using System.Timers;

namespace NetworkCryptography.App
{
    internal static class CoreApp<T> where T : IAppProvider, new()
    {
        public static T AppProvider { get; private set; }

        private const double TickInterval = 0.0166666667;
        private static readonly Timer tickTimer;

        static CoreApp()
        {
            tickTimer = new Timer(TickInterval);
            tickTimer.Elapsed += (sender, args) => AppProvider.Tick();
        }

        public static void Run()
        {
            AppProvider = new T();
            AppProvider.Initialize();

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
                    CoreApp<Client>.Run();

                    Console.Clear();
                    string ip = ConsoleDisplay.InputField("Please enter the IP address of the server you would like to connect to");
                    string port = ConsoleDisplay.InputField($"{Environment.NewLine}Please enter the port");

                    CoreApp<Client>.AppProvider.Connect(ip, int.Parse(port));
                    Console.Clear();

                    break;
                case 1:
                    Console.WriteLine("Starting server...");
                    CoreApp<Server>.Run();
                    CoreApp<Server>.AppProvider.Start();
                    break;
            }

            Console.ReadKey();
        }
    }
}
