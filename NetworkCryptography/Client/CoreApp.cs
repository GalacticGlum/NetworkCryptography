using System.Timers;
using NetworkCryptography.Core;

namespace NetworkCryptography.Client
{
    public static class CoreApp
    {
        public static Client Client { get; private set; }
        private static TickLoop tickLoop;

        static CoreApp()
        {
            tickLoop = new TickLoop(Tick);
        }

        public static void Run(string username, string ip, int port)
        {
            Client = new Client();
            Client.Connect(ip, port);
        }

        private static void Tick(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Client?.Tick();
        }
    }
}
