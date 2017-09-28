using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Timers;
using NetworkCryptography.Core.Helpers;

namespace NetworkCryptography.Core.Logging
{
    public static class Logger
    {
        public const string AllowAnyCategoryVerbosities = "__ALLOW_ANY_CATEGORY_VERBOSITIES__";

        public static LoggerVerbosity Verbosity { get; set; }
        public static LoggerDestination Destination { get; set; } = LoggerDestination.File;

        public static string LineSeperator { get; set; } = string.Empty;
        public static int LineSeperatorMessageInterval { get; set; } = -1;

        /// <summary>
        /// When a message with this verbosity (or higher) is logged, the whole message buffer is flushed.
        /// </summary>
        public static LoggerVerbosity FlushVerbosity { get; set; } = LoggerVerbosity.Error;
        /// <summary>
        /// This is the time (in seconds) after which we flush our log to the file.
        /// </summary>
        public static int FlushFrequency { get; set; } = 5;
        /// <summary>
        /// This is the amount of messages the message buffer can store until it must flush itself.
        /// </summary>
        public static int FlushBufferMessageCapacity { get; set; } = 100;
        /// <summary>
        /// The category verbosity filter. If set to null, then the filter will allow all categories.
        /// </summary>
        public static Dictionary<string, LoggerVerbosity> CategoryVerbosities { get; set; }
        public static int MessageCount { get; private set; }

        private static readonly StringBuilder messageBuffer = new StringBuilder();
        private static readonly Timer messageBufferFlushTimer;

        private static string logFilePath;
        private static bool isFirstLog = true;
        private static int messageCountSinceLastFlush;

        static Logger()
        {
            CategoryVerbosities = new Dictionary<string, LoggerVerbosity>
            {
                // Second parameter doesn't matter, if the key: AllowAnyCategoryVerbosities is in the dictionary then it just logs any category.
                {
                    AllowAnyCategoryVerbosities, LoggerVerbosity.None
                }
            };

            logFilePath = GetLogFilePath();

            // Initialize message buffer flush timer
            messageBufferFlushTimer = new Timer(FlushFrequency * 1000)
            {
                AutoReset = true
            };

            messageBufferFlushTimer.Elapsed += (sender, args) => FlushMessageBuffer();
            messageBufferFlushTimer.Start();

            // Clear message buffer if exists.
            if (File.Exists(logFilePath))
            {
                File.Delete(logFilePath);
            }

            // Print format
            const string format = "[yyyy-MM-dd HH:mm:ss.fff][verbosity] category: message";
            if ((Destination & LoggerDestination.File) != 0)
            {
                messageBuffer.AppendLine(format);
                messageBuffer.AppendLine(StringHelper.Overline.Multiply(format.Length));
            }

            if ((Destination & LoggerDestination.Output) == 0) return;

            Console.WriteLine(format);
            Console.WriteLine(StringHelper.Overline.Multiply(format.Length));
        }

        public static void Log(string category, object message, LoggerVerbosity messageVerbosity = LoggerVerbosity.Info, bool seperateLineHere = false)
        {
            if (messageVerbosity == LoggerVerbosity.None) return;
            lock (Console.Out)
            {
                if (Verbosity > messageVerbosity) return;
                if (CategoryVerbosities != null)
                {
                    if (!CategoryVerbosities.ContainsKey(category) && !CategoryVerbosities.ContainsKey(AllowAnyCategoryVerbosities)) return;
                    if (CategoryVerbosities.ContainsKey(category))
                    {
                        if (CategoryVerbosities[category] > messageVerbosity) return;
                    }
                }

                MessageCount++;

                string output = string.Concat(GetMessageHeader(messageVerbosity, category), message);
                string messageSeperator = string.Empty;
                if (!string.IsNullOrEmpty(LineSeperator) && (seperateLineHere || LineSeperatorMessageInterval > 0 &&
                                                             MessageCount % LineSeperatorMessageInterval == 0))
                {
                    messageSeperator = LineSeperator.Multiply(output.Length);
                }

                if ((Destination & LoggerDestination.File) != 0)
                {
                    messageBuffer.AppendLine(output);
                    if (!string.IsNullOrEmpty(messageSeperator))
                    {
                        messageBuffer.AppendLine(messageSeperator);
                    }
                }

                if ((Destination & LoggerDestination.Output) != 0)
                {
                    ConsoleColor oldConsoleColor = Console.ForegroundColor;
                    Console.ForegroundColor = GetConsoleColour(messageVerbosity);

                    if (messageVerbosity == LoggerVerbosity.Plain)
                    {
                        Console.WriteLine(message);
                    }
                    else
                    {
                        Console.WriteLine(output);
                    }

                    if (!string.IsNullOrEmpty(messageSeperator))
                    {
                        Console.WriteLine(messageSeperator);
                    }

                    Console.ForegroundColor = oldConsoleColor;
                }

                messageCountSinceLastFlush++;
                if (messageVerbosity >= FlushVerbosity || messageCountSinceLastFlush == FlushBufferMessageCapacity)
                {
                    FlushMessageBuffer();
                }
            }

            if (!isFirstLog) return;
            messageBufferFlushTimer.Start();
            isFirstLog = false;
        }

        public static void Log(object message, LoggerVerbosity messageVerbosity = LoggerVerbosity.Info)
        {
            Log(string.Empty, message, messageVerbosity);
        }

        public static void LogFunctionEntry(string category = "", string message = "", LoggerVerbosity messageVerbosity = LoggerVerbosity.Info, bool seperateLineHere = false)
        {
            MethodBase methodFrame = new StackTrace().GetFrame(1).GetMethod();
            string functionName = methodFrame.Name;
            string className = "NotAvailable";
            if (methodFrame.DeclaringType != null)
            {
                className = methodFrame.DeclaringType.FullName;
            }

            string messageContents = string.IsNullOrEmpty(message) ? string.Empty : $": {message}";
            string actualMessage = $"{className}::{functionName}{messageContents}";

            Log(category, actualMessage, messageVerbosity, seperateLineHere);
        }

        public static void LogFormat(string category, string message, LoggerVerbosity messageVerbosity = LoggerVerbosity.Info, bool seperateLineHere = false, params object[] args)
        {
            Log(category, string.Format(message, args), messageVerbosity, seperateLineHere);
        }

        public static void LogFormat(string message, LoggerVerbosity messageVerbosity = LoggerVerbosity.Info, bool seperateLineHere = false, params object[] args)
        {
            Log(string.Empty, string.Format(message, args), messageVerbosity, seperateLineHere);
        }

        public static void Assert(bool condition, string message = null, [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (condition) return;

            string assertMessage = $"Assert failed! {sourceFilePath} at line {sourceLineNumber}";
            message = !string.IsNullOrEmpty(message) ? $"{assertMessage}{Environment.NewLine}{message}" : assertMessage;

            Log(message, LoggerVerbosity.Error);
            Environment.Exit(-1);
        }

        private static string GetMessageHeader(LoggerVerbosity verbosity, string category)
        {
            string padding = StringHelper.Space.Multiply(GetLongestVerbosityLength() - GetVerbosityName(verbosity).Length);
            string verbosityHeader = $"[{padding}{GetVerbosityName(verbosity)}]";

            string header = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}]{verbosityHeader}";
            string categoryHeader = !string.IsNullOrEmpty(category) ? $" {category}: " : ": ";
            return string.Concat(header, categoryHeader);
        }

        private static ConsoleColor GetConsoleColour(LoggerVerbosity verbosity)
        {
            switch (verbosity)
            {
                case LoggerVerbosity.Info:
                    return ConsoleColor.White;
                case LoggerVerbosity.Warning:
                    return ConsoleColor.Yellow;
                case LoggerVerbosity.Error:
                    return ConsoleColor.Red;
            }

            return ConsoleColor.Gray;
        }

        private static string GetVerbosityName(LoggerVerbosity verbosity)
        {
            return Enum.GetName(typeof(LoggerVerbosity), verbosity);
        }

        public static void FlushMessageBuffer(int tries = 0)
        {
            if (tries > 2) return;
            if ((Destination & LoggerDestination.File) == 0) return;

            string flushContents = messageBuffer.ToString();
            if (messageCountSinceLastFlush == 0 || string.IsNullOrEmpty(flushContents)) return;

            try
            {
                File.AppendAllText(logFilePath, flushContents);
            }
            catch (IOException)
            {
                // The file is already being used (presumably), let's change the logFilePath and try again
                logFilePath = GetLogFilePath("2");
                FlushMessageBuffer(tries + 1);
            }

            messageCountSinceLastFlush = 0;
            messageBuffer.Clear();
        }

        private static int GetLongestVerbosityLength()
        {
            string[] verbosityNames = Enum.GetNames(typeof(LoggerVerbosity));
            return verbosityNames.OrderByDescending(str => str.Length).First().Length;
        }

        private static string GetLogFilePath(string suffix = null)
        {
            return $"{Assembly.GetEntryAssembly().GetName().Name}{suffix}.log";
        }
    }
}

