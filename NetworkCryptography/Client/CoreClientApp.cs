/*
 * Author: Shon Verch
 * File Name: CoreClientApp.cs
 * Project: NetworkCryptography
 * Creation Date: 9/27/2017
 * Modified Date: 10/17/2017
 * Description: The main application context; manages all logic.
 */

using System;
using System.Timers;
using NetworkCryptography.Core;
using NetworkCryptography.Core.Helpers;
using NetworkCryptography.Core.Logging;
using NetworkCryptography.Core.Networking;

namespace NetworkCryptography.Client
{
    /// <summary>
    /// Main application context; manages all logic.
    /// </summary>
    public static class CoreClientApp
    {
        /// <summary>
        /// Is the application still running.
        /// </summary>
        public static bool IsRunning => tickLoop.Running;

        /// <summary>
        /// Networked client peer.
        /// </summary>
        public static Client Client { get; }

        /// <summary>
        /// The cryptography method which the server utilises.
        /// </summary>
        public static CryptographyMethodType SelectedCryptographyMethodType { get; private set; }

        /// <summary>
        /// The cryptography engine that is used for decrypting and encrypting messages.
        /// </summary>
        public static ICryptographicMethod CryptographicEngine { get; private set; }
        
        /// <summary>
        /// The logic loop ticker.
        /// </summary>
        private static readonly TickLoop tickLoop;

        static CoreClientApp()
        {
            Client = new Client();
            tickLoop = new TickLoop(Tick);
        }

        /// <summary>
        /// Starts up the client and connects to a server.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public static void Run(string username, string ip, int port)
        {
            Client.Connect(username, ip, port);
            
            tickLoop.Start();
            Logger.Destination = LoggerDestination.All;
        }

        /// <summary>
        /// Handle quit logic.
        /// </summary>
        public static void Quit()
        {
            Logger.FlushMessageBuffer();
            Client.Disconnect();

            tickLoop.Stop();
        }

        /// <summary>
        /// Handle the SendCryptographyMethodType message from the server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void HandleCryptographyMethodType(object sender, PacketRecievedEventArgs args)
        {
            Logger.LogFunctionEntry();

            SelectedCryptographyMethodType = (CryptographyMethodType) args.Message.ReadByte();
            CryptographicEngine = CryptographyHelper.CreateEngine(SelectedCryptographyMethodType);
        }

        /// <summary>
        /// Runs continuous logic for the application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="elapsedEventArgs"></param>
        private static void Tick(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Client?.Tick();
        }
    }
}
