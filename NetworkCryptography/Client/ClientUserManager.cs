/*
 * Author: Shon Verch
 * File Name: UserManager.cs
 * Project Name: NetworkCryptography
 * Creation Date: 10/17/2017
 * Modified Date: 10/17/2017
 * Description: Manages all users connected to the server.
 */

using NetworkCryptography.Core;
using NetworkCryptography.Core.Logging;
using NetworkCryptography.Core.Networking;

namespace NetworkCryptography.Client
{
    /// <summary>
    /// Manages all users connected to the server.
    /// </summary>
    public class ClientUserManager : UserManager
    {
        /// <summary>
        /// Reads the list of users sent from the server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void HandleUserListPacket(object sender, PacketRecievedEventArgs args)
        {
            int count = args.Buffer.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                int id = args.Buffer.ReadInt32();
                string name = args.Buffer.ReadString();

                Logger.Log(name);
                Add(id, name);
            }
        }

        /// <summary>
        /// Clears the user manager database.
        /// </summary>
        public void Clear()
        {
            users.Clear();
        }
    }
}
