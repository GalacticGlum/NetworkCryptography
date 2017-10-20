/*
 * Author: Shon Verch
 * File Name: UserManager.cs
 * Project Name: NetworkCryptography
 * Creation Date: 10/17/2017
 * Modified Date: 10/20/2017
 * Description: Manages all users connected to the server.
 */

using System;
using System.Collections.Generic;
using NetworkCryptography.Client.Pages;
using NetworkCryptography.Core;
using NetworkCryptography.Core.Logging;
using NetworkCryptography.Core.Networking;

namespace NetworkCryptography.Client
{
    /// <summary>
    /// Event for whenever a new user joins.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void NewUserJoinedEventHandler(object sender, UserEventArgs args);

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
    /// Event for whenever we receive a user list (when we first connect).
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void UserListReceivedEventHandler(object sender, UserListEventArgs args);

    /// <summary>
    /// Event arguments for a collection of users.
    /// </summary>
    public class UserListEventArgs : EventArgs
    {
        /// <summary>
        /// Collection of users joined.
        /// </summary>
        public IEnumerable<User> Users { get; }

        /// <summary>
        /// Creates an instance of UserListEventArgs with a specified collection of users.
        /// </summary>
        /// <param name="users"></param>
        public UserListEventArgs(IEnumerable<User> users)
        {
            Users = users;
        }
    }

    /// <summary>
    /// Manages all users connected to the server.
    /// </summary>
    public class ClientUserManager : UserManager
    {
        public User BelongingUser { get; private set; }

        /// <summary>
        /// Event which is raised whenever we receive a user list.
        /// </summary>
        public event UserListReceivedEventHandler UserListReceived;

        /// <summary>
        /// Raises the UserListReceived event.
        /// </summary>
        /// <param name="userList"></param>
        private void OnUserListReceived(IEnumerable<User> userList)
        {
            UserListReceived?.Invoke(this, new UserListEventArgs(userList));
        }

        /// <summary>
        /// Event which is raised whenever a new user joins.
        /// </summary>
        public event NewUserJoinedEventHandler NewUserJoined;

        /// <summary>
        /// Raises the NewUserJoined event.
        /// </summary>
        /// <param name="user"></param>
        private void OnNewUserJoined(User user)
        {
            NewUserJoined?.Invoke(this, new UserEventArgs(user));
        }

        /// <summary>
        /// Reads the list of users sent from the server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void HandleUserListPacket(object sender, PacketRecievedEventArgs args)
        {
            int count = args.Message.ReadInt32();
            User[] userList = new User[count];

            for (int i = 0; i < count; i++)
            {
                int id = args.Message.ReadInt32();
                string name = args.Message.ReadString();
                User user = new User(id, name);

                Add(user);
                userList[i] = user;
            }

            OnUserListReceived(userList);
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

            User user = new User(id, name);
            Add(user);

            OnNewUserJoined(user);

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
