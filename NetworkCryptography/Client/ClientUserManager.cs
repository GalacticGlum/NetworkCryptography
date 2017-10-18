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
        public User BelongingUser { get; private set; }

        /// <summary>
        /// Reads the list of users sent from the server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void HandleUserListPacket(object sender, PacketRecievedEventArgs args)
        {
            int count = args.Message.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                int id = args.Message.ReadInt32();
                string name = args.Message.ReadString();

                Add(id, name);
            }
        }

        /// <summary>
        /// Handles the SendBelongingUserToClient message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void ReceiveBelongingUser(object sender, PacketRecievedEventArgs args)
        {
            int id = args.Message.ReadInt32();
            BelongingUser = this[id];
        }

        /// <summary>
        /// Handle a user joining.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void HandleNewUser(object sender, PacketRecievedEventArgs args)
        {
            int id = args.Message.ReadInt32();
            string name = args.Message.ReadString();

            Add(id, name);

            Logger.Log($"{name} joined.");
        }

        /// <summary>
        /// Handle a user leaving.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void HandleUserLeft(object sender, PacketRecievedEventArgs args)
        {
            int id = args.Message.ReadInt32();
            User user = Remove(id);

            Logger.Log($"{user.Name} left");
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
