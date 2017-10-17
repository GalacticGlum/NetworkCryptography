/*
 * Author: Shon Verch
 * File Name: ServerOutgoingPacketType.cs
 * Project: NetworkCryptography
 * Creation Date: 9/25/2017
 * Modified Date: 9/25/2017
 * Description: A packet which leaves from the server and arrives at a client.
 */

namespace NetworkCryptography.Core.Networking
{
    /// <summary>
    /// A packet which leaves from the server and arrives at a client.
    /// </summary>
    public enum ServerOutgoingPacketType
    {
        SendUserList,
        SendNewUser,
        RelayMessage,
        Pong
    }
}
