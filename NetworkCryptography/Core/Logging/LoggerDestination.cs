/*
 * Author: Shon Verch
 * File Name: LoggerDestination.cs
 * Project: NetworkCryptography
 * Creation Date: 9/22/2017
 * Modified Date: 9/22/2017
 * Description: The different types of destinations the Logger can log to.
 */

using System;
using System.Diagnostics;

namespace NetworkCryptography.Core.Logging
{
    /// <summary>
    /// The different types of destinations the <see cref="Logger"/> can log to.
    /// </summary>
    [Flags]
    public enum LoggerDestination
    {
        /// <summary>
        /// Do not log to anywhere.
        /// </summary>
        None = 0,

        /// <summary>
        /// Only log to the standard output (<see cref="Console"/> or <see cref="Trace"/>).
        /// </summary>
        Output = 1,

        /// <summary>
        /// Only log to file.
        /// </summary>
        File = 2,

        /// <summary>
        /// Log to both file and standard output.
        /// </summary>
        All = File | Output
    }
}
