﻿/*
 * Author: Shon Verch
 * File Name: Server.cs
 * Project: NetworkCryptography
 * Creation Date: 9/25/2017
 * Modified Date: 10/18/2017
 * Description: The server peer; handles all server-side networking.
 */

using System.Collections.Generic;
using Lidgren.Network;
using NetworkCryptography.Core;
using NetworkCryptography.Core.Helpers;
using NetworkCryptography.Core.Logging;
using NetworkCryptography.Core.Networking;

namespace NetworkCryptography.Server
{
    /// <summary>
    /// The server peer; handles all server-side networking.
    /// </summary>
    public class Server : Peer<NetServer>
    {
        /// <summary>
        /// The port which the server runs on.
        /// </summary>
        public int ServerPort { get; }

        /// <summary>
        /// The maximum amount of connections allowed on the server.
        /// </summary>
        public int MaximumConnections { get; }

        /// <summary>
        /// Manages users on the server.
        /// </summary>
        public ServerUserManager UserManager { get; }

        /// <summary>
        /// Manages chat message on the server.
        /// </summary>
        public ServerChatMessageManager ChatMessageManager { get; }

        /// <summary>
        /// Indicates whether the server is running.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Creates the server with a specified port and maximum connections.
        /// </summary>
        /// <param name="port">The port which the server will run on.</param>
        /// <param name="maximumConnections">The maximum amount of connections allowed on this server (at the same time).</param>
        public Server(int port, int maximumConnections = 100)
        {
            ServerPort = port;
            MaximumConnections = maximumConnections;

            UserManager = new ServerUserManager();
            ChatMessageManager = new ServerChatMessageManager(Packets);

            Packets[ClientOutgoingPacketType.Ping] += HandlePingRequest;
        }

        /// <summary>
        /// Create the underlaying Lidgren net server.
        /// </summary>
        /// <returns></returns>
        protected override NetServer ConstructPeer()
        {
            NetConfiguration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            return new NetServer(NetConfiguration);
        }

        /// <summary>
        /// Create the underlaying Lidgren net configuration.
        /// </summary>
        /// <returns></returns>
        protected override NetPeerConfiguration ConstructNetPeerConfiguration()
        {
            return new NetPeerConfiguration("airballoon")
            {
                Port = ServerPort,
                MaximumConnections = MaximumConnections
            };
        }

        /// <summary>
        /// Start the server.
        /// </summary>
        public void Start()
        {
            Validate();
            HandleMessageType(NetIncomingMessageType.ConnectionApproval, HandleConnectionApproval);

            PeerConnected += OnUserConnected;
            PeerDisconnected += OnUserDisconnected;

            NetPeer.Start();
            IsRunning = true;
        }

        /// <summary>
        /// Handles a ping request.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void HandlePingRequest(object sender, PacketRecievedEventArgs args)
        {
            Send(ServerOutgoingPacketType.Pong, args.SenderConnection, NetDeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Handle a user connection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnUserConnected(object sender, ConnectionEventArgs args)
        {
            string username = args.Connection.RemoteHailMessage.ReadString();
            Logger.Log($"{username} connected from remote endpoint: {args.Connection.RemoteEndPoint.Address}", LoggerVerbosity.Plain);

            User newUser = new User(UserManager.Count, username);
            UserManager.Add(newUser);

            SendCryptographyMethodType(args.Connection);
            UserManager.SendUserList(args.Connection);
            UserManager.SendBelongingUserToClient(newUser, args.Connection);
            ChatMessageManager.SendMessageHistory(args.Connection);

            SendUserJoined(newUser);
        }

        /// <summary>
        /// Sends the cryptographic method type that the server is using to a target client.
        /// </summary>
        /// <param name="target">The target client to send to.</param>
        private void SendCryptographyMethodType(NetConnection target)
        {
            NetBuffer messageBuffer = CreatePacket(ServerOutgoingPacketType.SendCryptographyMethodType);
            messageBuffer.Write((byte)CoreServerApp.SelectedCryptographyMethodType);
            Send(messageBuffer, target, NetDeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Handle connection approval.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void HandleConnectionApproval(object sender, IncomingMessageEventArgs args)
        {
            args.Message.SenderConnection.Approve();
        }

        /// <summary>
        /// Handle user disconnected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnUserDisconnected(object sender, ConnectionEventArgs args)
        {
            int id = int.Parse(args.Message.ReadString());
            SendUserLeft(id);

            User user = UserManager.Remove(id);
            Logger.Log($"{user.Name} disconnected!", LoggerVerbosity.Plain);
        }

        /// <summary>
        /// Notify all clients that a user has joined.
        /// </summary>
        /// <param name="user"></param>
        private void SendUserJoined(User user)
        {
            NetBuffer message = CreatePacket(ServerOutgoingPacketType.SendUserJoined);
            message.Write(user.Id);
            message.Write(user.Name);

            SendToAll(message, NetDeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Notify all clients that a user has left.
        /// </summary>
        /// <param name="id"></param>
        private void SendUserLeft(int id)
        {
            NetBuffer message = CreatePacket(ServerOutgoingPacketType.SendUserLeft);
            message.Write(id);
            SendToAll(message, NetDeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Stop the server.
        /// </summary>
        public void Stop()
        {
            NetPeer.Shutdown(string.Empty);
            IsRunning = false;
        }

        /// <summary>
        /// Send a buffer of data to a specific connection.
        /// </summary>
        /// <param name="packet">The buffer of data to send.</param>
        /// <param name="connection">The connection to send the data to.</param>
        /// <param name="deliveryMethod">How should the data be delivered.</param>
        public void Send(NetBuffer packet, NetConnection connection, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.Unreliable)
        {
            NetPeer.SendMessage((NetOutgoingMessage)packet, connection, deliveryMethod);
        }

        /// <summary>
        /// Send a buffer of data to a list of connections.
        /// </summary>
        /// <param name="packet">The buffer of data to send.</param>
        /// <param name="connections">The list of connections to send the data to.</param>
        /// <param name="deliveryMethod">How should the data be delivered.</param>
        public void Send(NetBuffer packet, IList<NetConnection> connections, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.Unreliable)
        {
            foreach (NetConnection netConnection in connections)
            {
                NetPeer.SendMessage((NetOutgoingMessage)packet, netConnection, deliveryMethod);
            }
        }

        /// <summary>
        /// Send a specific header to a connection.
        /// </summary>
        /// <param name="header">The header to send.</param>
        /// <param name="connection">The connection to send the header to.</param>
        /// <param name="deliveryMethod">How should the data be delivered.</param>
        public void Send(ServerOutgoingPacketType header, NetConnection connection, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.Unreliable)
        {
            NetPeer.SendMessage(CreateMessageWithHeader((int)header), connection, deliveryMethod);
        }

        /// <summary>
        /// Send a specific header to a list of connections.
        /// </summary>
        /// <param name="header">The header to send.</param>
        /// <param name="connections">The list of connections to send the header to.</param>
        /// <param name="deliveryMethod">How should the data be delivered.</param>
        public void Send(ServerOutgoingPacketType header, IList<NetConnection> connections, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.Unreliable)
        {
            foreach (NetConnection netConnection in connections)
            {
                NetPeer.SendMessage(CreateMessageWithHeader((int)header), netConnection, deliveryMethod);
            }
        }

        /// <summary>
        /// Send a buffer of data to all clients connected to the server.
        /// </summary>
        /// <param name="packet">The buffer of data to send.</param>
        /// <param name="deliveryMethod">How should the data be delivered.</param>
        public void SendToAll(NetBuffer packet, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.Unreliable)
        {
            NetPeer.SendToAll((NetOutgoingMessage)packet, deliveryMethod);
        }

        /// <summary>
        /// Send a buffer of data to all clients connected to the server.
        /// </summary>
        /// <param name="header">The header to send.</param>
        /// <param name="deliveryMethod">How should the data be delivered.</param>
        public void SendToAll(ServerOutgoingPacketType header, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.Unreliable)
        {
            NetPeer.SendToAll(CreateMessageWithHeader((int)header), deliveryMethod);
        }

        /// <summary>
        /// Prints the names of all the connected users.
        /// </summary>
        public void PrintConnectedUsers()
        {
            // Print header
            bool isOneUserConnected = UserManager.Count == 1;
            string usersConnectedMessage = $"There {(isOneUserConnected ? "is" : "are")} {UserManager.Count} {(isOneUserConnected ? "user" : "users")} connected.";
            Logger.Log(usersConnectedMessage, LoggerVerbosity.Plain);
            Logger.Log(StringHelper.Overline.Multiply(usersConnectedMessage.Length), LoggerVerbosity.Plain);

            // Print name of each user
            foreach (User user in UserManager)
            {
                Logger.Log(user.Name, LoggerVerbosity.Plain);
            }

            Logger.Log(string.Empty, LoggerVerbosity.Plain);
        }

        /// <summary>
        /// Run update logic for the server.
        /// </summary>
        public void Tick()
        {
            Listen();
        }
    }
}
