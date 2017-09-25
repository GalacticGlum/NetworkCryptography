using System;
using System.Net;
using System.Net.Sockets;

namespace NetworkCryptography.Core.Helpers
{
    public static class NetworkHelper
    {
        public static IPAddress GetLocalIpAddress()
        {
            IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ipAddress in hostEntry.AddressList)
            {
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ipAddress;
                }
            }

            throw new Exception("Could not find local IP address");
        }
    }
}
