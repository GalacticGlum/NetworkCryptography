/*
 * Author: Shon Verch
 * File Name: CoreServerApp.cs
 * Project: NetworkCryptography
 * Creation Date: 9/27/2017
 * Modified Date: 10/18/2017
 * Description: The main application context; manages all logic.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Timers;
using NetworkCryptography.Core;
using NetworkCryptography.Core.DataStructures;
using NetworkCryptography.Core.Helpers;
using NetworkCryptography.Core.Logging;

namespace NetworkCryptography.Server
{
    /// <summary>
    /// The main application context; manages all logic.
    /// </summary>
    internal static class CoreServerApp
    {
        /// <summary>
        /// Indicates whether the application context is running.
        /// </summary>
        public static bool IsRunning => tickLoop.Running;

        /// <summary>
        /// Networked server peer.
        /// </summary>
        public static Server Server { get; private set; }

        /// <summary>
        /// Server settings file.
        /// </summary>
        public static Settings Settings { get; }

        /// <summary>
        /// Indicates whether a command is currently being executed.
        /// </summary>
        public static bool IsCommandRunning { get; private set; }

        /// <summary>
        /// The selected cryptography method type.
        /// </summary>
        public static CryptographyMethodType SelectedCryptographyMethodType { get; private set; }

        /// <summary>
        /// The title of the server console window.
        /// </summary>
        private const string WindowTitle = "Airballoon Server";

        /// <summary>
        /// The logic loop ticker.
        /// </summary>
        private static readonly TickLoop tickLoop;

        /// <summary>
        /// Properly formatted names of different cryptography methods.
        /// </summary>
        private static readonly string[] cryptographyTypeNames =
        {
            "Caesar Cipher",
            "Data Encryption Standard (DES)",
            "Rivest–Shamir–Adleman (RSA) Scheme"
        };

        static CoreServerApp()
        {
            Console.Title = WindowTitle;

            Settings = Settings.Load();
            tickLoop = new TickLoop(Tick);
        }

        /// <summary>
        /// Starts up the server on the specified port with the specified cryptography method.
        /// </summary>
        /// <param name="port"></param>
        public static void Run(int port)
        {
            Logger.Destination = LoggerDestination.All;

            string message = "In cryptography, a Feistel cipher is a symmetric structure used in the construction of block ciphers, " +
                               "named after the German-born physicist and cryptographer Horst Feistel who did pioneering research while " +
                               "working for IBM (USA); it is also commonly known as a Feistel network. A large proportion of block ciphers " +
                               "use the scheme, including the Data Encryption Standard (DES). The Feistel structure has the advantage that " +
                               "encryption and decryption operations are very similar, even identical in some cases, requiring only a reversal " +
                               "of the key schedule. Therefore, the size of the code or circuitry required to implement such a cipher is nearly " +
                               "halved.\r\n\r\nA Feistel network is an iterated cipher with an internal function called a round function.[1]";

            RsaKeySet keys = new RsaKeySet(65537, 1919621681, 2114731571);

            RsaCryptographicMethod rsa = new RsaCryptographicMethod(keys);
            int[] ciphertext = rsa.Encrypt(message);
            ciphertext.Print();
            string plaintext = rsa.Decrypt(ciphertext);
            Logger.Log(plaintext);

            //PaddedBuffer ciphertext = CBCDes.Encrypt(message, 67);
            //string plaintext = CBCDes.Decrypt(ciphertext, 67);
            //Logger.Log(plaintext);

            Server = new Server(port);
            Server.Start();

            Initialize();

            string address = $"<{NetworkHelper.GetLocalIpAddress()}:{port}>";
            Logger.Log($"Started server on {address}", LoggerVerbosity.Plain);
            Console.Title = $"{WindowTitle} | {address}";
        }

        /// <summary>
        /// Run initialization logic for the application.
        /// </summary>
        private static void Initialize()
        {
            tickLoop.Start();
        }

        /// <summary>
        /// Run update logic for the application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="elapsedEventArgs"></param>
        private static void Tick(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Server?.Tick();
        }

        /// <summary>
        /// Process any server console commands.
        /// </summary>
        private static void ProcessConsoleCommands()
        {
            Logger.MessageLogged += args =>
            {
                if (IsCommandRunning) return;
                Console.Write(">>> ");
            };

            while (IsRunning)
            {
                Console.Write(">>> ");
                string line = Console.ReadLine();
                if (string.IsNullOrEmpty(line)) continue;

                IsCommandRunning = true;
                switch (line)
                {
                    case "quit":
                        Exit();
                        break;
                    case "clear":
                        Console.Clear();
                        break;
                    case "users":
                        Server.PrintConnectedUsers();
                        break;
                    case "write_history":
                        Logger.Log(Server.ChatMessageManager.Count, LoggerVerbosity.Plain);

                        StringBuilder builder = new StringBuilder();
                        foreach (SimplifiedChatMessage message in Server.ChatMessageManager)
                        {
                            string time = $"{DateTime.FromBinary(message.TimeInBinary):hh:mm:ss tt}";
                            builder.AppendLine($"{Server.UserManager[message.UserId].Name}: {message.Message} | {time}");
                        }

                        FileInfo file = new FileInfo($"./Logs/{DateTime.Now:dd_MM_yyyy}.log");
                        file.Directory?.Create();

                        File.WriteAllText(file.FullName, builder.ToString());

                        break;
                    default:
                        IsCommandRunning = false;
                        Console.WriteLine($"'{line}' is not recognized as a command.{Environment.NewLine}");
                        break;
                }

                IsCommandRunning = false;
            }
        }

        /// <summary>
        /// Close the application.
        /// </summary>
        private static void Exit()
        {
            tickLoop.Stop();
            Logger.FlushMessageBuffer();
        }

        /// <summary>
        /// Main entry point for the application.
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            const int maxPortValue = 65535;

            int port;
            if (Settings == null || Settings.Port <= 0 || Settings.Port >= maxPortValue)
            {
               port = int.Parse(ConsoleDisplay.InputField("Please enter the port you would like to run the server on."));
            }
            else
            {
                port = Settings.Port;
            }

            Console.Clear();

            SelectedCryptographyMethodType = EnumHelper.GetValue<CryptographyMethodType>(ConsoleDisplay.Menu(
                "What cryptography method would you like to use?", cryptographyTypeNames));

            Console.Clear();
            
            Run(port);
            ProcessConsoleCommands();
        }
    }
}
