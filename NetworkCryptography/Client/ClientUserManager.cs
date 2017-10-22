/*
 * Author: Shon Verch
 * File Name: ClientUserManager.cs
 * Project Name: NetworkCryptography
 * Creation Date: 10/17/2017
 * Modified Date: 10/20/2017
 * Description: Manages all users connected to the server.
 */

using System;
using NetworkCryptography.Core;
using NetworkCryptography.Core.Networking;

namespace NetworkCryptography.Client
{
    /// <summary>
    /// Generic event handler for any user-related event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void UserEventHandler(object sender, UserEventArgs args);

    /// <summary>
    /// Generic event arguments for any user event.
    /// </summary>
    public class UserEventArgs : EventArgs
    {
        /// <summary>
        /// The new user who has joined.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Creates an instance of UserEventArgs with a specified user.
        /// </summary>
        /// <param name="user"></param>
        public UserEventArgs(User user)
        {
            User = user;
        }
    }

    /// <summary>
    /// Manages all users connected to the server.
    /// </summary>
    public class ClientUserManager : UserManager
    {
        /// <summary>
        /// The user which belongs to this client.
        /// </summary>
        public User BelongingUser { get; private set; }

        /// <summary>
        /// Event which is raised whenever a new user joins.
        /// </summary>
        public event UserEventHandler NewUserJoined;

        /// <summary>
        /// Raises the NewUserJoined event.
        /// </summary>
        /// <param name="user"></param>
        private void OnNewUserJoined(User user)
        {
            NewUserJoined?.Invoke(this, new UserEventArgs(user));
        }

        /// <summary>
        /// Event which is raised whenever a user leaves.
        /// </summary>
        public event UserEventHandler UserLeft;

        /// <summary>
        /// Raises the UserLeft event.
        /// </summary>
        /// <param name="user"></param>
        private void OnUserLeft(User user)
        {
            UserLeft?.Invoke(this, new UserEventArgs(user));
        }

        /// <summary>
        /// Event which is raised when the belonging user is sent from the server.
        /// </summary>
        public event UserEventHandler BelongingUserReceived;

        /// <summary>
        /// Raises the BelongingUserReceived event.
        /// </summary>
        private void OnBelongingUserReceived()
        {
            BelongingUserReceived?.Invoke(this, new UserEventArgs(BelongingUser));
        }

        /// <summary>
        /// Initializes the <see cref="ClientUserManager"/> and all events.
        /// </summary>
        /// <param name="packetHandler"></param>
        public ClientUserManager(PacketHandlerCollection packetHandler)
        {
            packetHandler[ServerOutgoingPacketType.SendBelongingUserToClient] += ReceiveBelongingUser;
            packetHandler[ServerOutgoingPacketType.SendUserList] += HandleUserListPacket;
            packetHandler[ServerOutgoingPacketType.SendUserJoined] += HandleNewUser;
            packetHandler[ServerOutgoingPacketType.SendUserLeft] += HandleUserLeft;
        }

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
                bool isOffline = args.Message.ReadBoolean();

                User user = new User(id, name, isOffline);

                Add(user);
                if (!isOffline)
                {
                    OnNewUserJoined(user);
                }
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

            OnBelongingUserReceived();
        }

        /// <summary>
        /// Handle a user joining.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void HandleNewUser(object sender, PacketRecievedEventArgs args)
        {
            int id = args.Message.ReadInt32();
            if (id == BelongingUser.Id) return;

            string name = args.Message.ReadString();

            User user = new User(id, name);
            Add(user);

            OnNewUserJoined(user);
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

            OnUserLeft(user);
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
