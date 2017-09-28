namespace NetworkCryptography.Core.Logging
{
    public enum LoggerVerbosity
    {
        /// <summary>
        /// None verbosity will not log anything if used.
        /// </summary>
        None, // Not really a verbosity rather a way of internally defining: "print any verbosity".

        /// <summary>
        /// Plain verbosity, this will log text without any timestamp, category, etc...
        /// </summary>
        Plain,

        /// <summary>
        /// Regular message.
        /// </summary>
        Info,

        /// <summary>
        /// Warning message. 
        /// </summary>
        Warning,

        /// <summary>
        /// Error message.
        /// </summary>
        Error
    }
}
