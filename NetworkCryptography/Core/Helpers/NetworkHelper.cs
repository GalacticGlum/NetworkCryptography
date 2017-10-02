/*
 * Author: Shon Verch
 * File Name: NetworkHelper.cs
 * Project: NetworkCryptography
 * Creation Date: 9/25/2017
 * Modified Date: 9/25/2017
 * Description: Collection of useful network-related functionality.
 */

using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace NetworkCryptography.Core.Helpers
{
    /// <summary>
    /// Collection of useful network-related functionality.
    /// </summary>
    public static class NetworkHelper
    {
        /// <summary>
        /// Retrieve the local IP address of this machine.
        /// </summary>
        /// <returns>The local IP address.</returns>
        public static IPAddress GetLocalIpAddress()
        {
            IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());

            // Retrieve the first IP address with a InterNetwork family. If there is none then we return the default value
            // of IPAddress: default(IPAddress)
            return hostEntry.AddressList.FirstOrDefault(address => address.AddressFamily == AddressFamily.InterNetwork);
        }
    }
}
