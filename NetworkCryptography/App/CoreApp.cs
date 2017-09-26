using System.Timers;

namespace NetworkCryptography.App
{
    internal static class CoreApp
    {
        public static Client Client { get; private set; }
        public static Server Server { get; private set; }

        private const double TickInterval = 0.0166666667;
        private static readonly Timer tickTimer;

        static CoreApp()
        {
            tickTimer = new Timer(TickInterval);
            tickTimer.Elapsed += (sender, args) => Tick();
        }

        public static void RunAsClient(string ip, int port)
        {
            // If a client or server has already been initialized, we can't re-initialize!
            if (Client != null || Server != null) return;

            Client = new Client();
            Client.Connect(ip, port);

            tickTimer.Start();
        }

        public static void RunAsServer(int port)
        {
            // If a client or server has already been initialized, we can't re-initialize!
            if (Client != null || Server != null) return;

            Server = new Server(port);
            Server.Start();

            tickTimer.Start();
        }

        private static void Tick()
        {
            Server?.Tick();
            Client?.Tick();
        }
    }
}
