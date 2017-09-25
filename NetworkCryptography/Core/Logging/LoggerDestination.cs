using System;

namespace NetworkCryptography.Core.Logging
{
    [Flags]
    public enum LoggerDestination
    {
        None = 0,
        Output = 1,
        File = 2,
        All = File | Output
    }
}
