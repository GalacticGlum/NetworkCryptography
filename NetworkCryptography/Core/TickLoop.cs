/*
 * Author: Shon Verch
 * File Name: TickLoop.cs
 * Project: NetworkCryptography
 * Creation Date: 9/28/2017
 * Modified Date: 9//28/2017
 * Description: Loop which ticks at a 60th of a second.
 */

using System.Timers;

namespace NetworkCryptography.Core
{
    /// <summary>
    /// Loop which ticks at a 60th of a second.
    /// </summary>
    public sealed class TickLoop
    {
        /// <summary>
        /// Is our loop running.
        /// </summary>
        public bool Running => tickTimer.Enabled;

        /// <summary>
        /// The interval at which our loop runs at: 60^-1
        /// </summary>
        private const double TickInterval = 0.0166666667;
        private readonly Timer tickTimer;

        /// <summary>
        /// Creates a loop with a specified tick event.
        /// </summary>
        /// <param name="tickAction">The tick event.</param>
        public TickLoop(ElapsedEventHandler tickAction)
        {
            tickTimer = new Timer(TickInterval)
            {
                AutoReset = true
            };

            tickTimer.Elapsed += tickAction;
        }

        /// <summary>
        /// Start the loop.
        /// </summary>
        public void Start() => tickTimer.Start();

        /// <summary>
        /// Stop the loop.
        /// </summary>
        public void Stop() => tickTimer.Stop();
    }
}
