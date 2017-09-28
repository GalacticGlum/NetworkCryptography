using System.Timers;

namespace NetworkCryptography.Core
{
    public sealed class TickLoop
    {
        public bool Running => tickTimer.Enabled;

        private const double TickInterval = 0.0166666667;
        private readonly Timer tickTimer;

        public TickLoop(ElapsedEventHandler tickAction)
        {
            tickTimer = new Timer(TickInterval)
            {
                AutoReset = true
            };

            tickTimer.Elapsed += tickAction;
        }

        public void Start()
        {
            tickTimer.Start();
        }

        public void Stop()
        {
            tickTimer.Stop();
        }
    }
}
