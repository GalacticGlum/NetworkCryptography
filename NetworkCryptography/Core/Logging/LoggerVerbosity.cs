namespace NetworkCryptography.Core.Logging
{
    public enum LoggerVerbosity
    {
        /// <summary>
        /// None verbosity will not log anything if used.
        /// </summary>
        None = 0, // Not really a verbosity rather a way of internally defining: "print any verbosity".

        /// <summary>
        /// Regular message.
        /// </summary>
        Info = 1,

        /// <summary>
        /// Warning message. 
        /// </summary>
        Warning = 2,

        /// <summary>
        /// Error message.
        /// </summary>
        Error = 3
    }
}
