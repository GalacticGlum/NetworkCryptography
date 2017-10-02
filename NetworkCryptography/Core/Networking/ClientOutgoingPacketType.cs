/*
 * Author: Shon Verch
 * File Name: ClientOutgoingPacketType.cs
 * Project: NetworkCryptography
 * Creation Date: 9/25/2017
 * Modified Date: 9/25/2017
 * Description: A packet which leaves from the client and arrives at the server.
 */

namespace NetworkCryptography.Core.Networking
{
    /// <summary>
    /// A packet which leaves from the client and arrives at the server.
    /// </summary>
    public enum ClientOutgoingPacketType
    {
        SendLogin,
        SendMessage
    }
}
