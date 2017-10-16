/*
 * Author: Shon Verch
 * File Name: Server.cs
 * Project: NetworkCryptography
 * Creation Date: 9/25/2017
 * Modified Date: 10/14/2017
 * Description: The server peer; handles all server-side networking.
 */

using System.Collections.Generic;
using Lidgren.Network;
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

            PeerDisconnected += OnUserDisconnected;
        }

        /// <summary>
        /// Handle user disconnected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void OnUserDisconnected(object sender, ConnectionEventArgs args)
        {
            Logger.Log("User Disconnected -- but we don't know who!");
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

            NetPeer.Start();
            IsRunning = true;
        }

        /// <summary>
        /// Handle connection approval.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void HandleConnectionApproval(object sender, IncomingMessageEventArgs args)
        {
            string username = args.Message.ReadString();
            Logger.Log($"{username} connected @ {args.Message.SenderConnection.RemoteEndPoint.Address}");

            args.Message.SenderConnection.Approve();
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
        /// Run update logic for the server.
        /// </summary>
        public void Tick()
        {
            Listen();
        }
    }
}
