/*
 * Author: Shon Verch
 * File Name: UserManager.cs
 * Project Name: NetworkCryptography
 * Creation Date: 10/17/2017
 * Modified Date: 10/17/2017
 * Description: Manages all users connected to the server.
 */

using Lidgren.Network;
using NetworkCryptography.Core;
using NetworkCryptography.Core.Networking;

namespace NetworkCryptography.Server
{
    /// <summary>
    /// Manages all users connected to the server.
    /// </summary>
    public sealed class ServerUserManager : UserManager
    {
        /// <summary>
        /// Sends the user list to a target client.
        /// </summary>
        public void SendUserList(NetConnection target)
        {
            NetBuffer message = CoreServerApp.Server.CreatePacket(ServerOutgoingPacketType.SendUserList);

            message.Write(Count);
            foreach (User user in users.Values)
            {
                message.Write(user.Id);
                message.Write(user.Name);
            }

            CoreServerApp.Server.Send(message, target, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
