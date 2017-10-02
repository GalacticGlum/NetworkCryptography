/*
 * Author: Shon Verch
 * File Name: Peer.cs
 * Project: NetworkCryptography
 * Creation Date: 9/25/2017
 * Modified Date: 9/25/2017
 * Description: Base class for all network peers.
 */

using System;
using System.Collections.Generic;
using System.Net;
using NetworkCryptography.Core.Logging;
using Lidgren.Network;

namespace NetworkCryptography.Core.Networking
{
    /// <summary>
    /// Event handler for network connections.
    /// </summary>
    /// <param name="sender">The object which raised the event.</param>
    /// <param name="args">The event arguments.</param>
    public delegate void ConnectionEventHandler(object sender, ConnectionEventArgs args);

    /// <summary>
    /// Stores all argument data for the connection event.
    /// </summary>
    public class ConnectionEventArgs : EventArgs
    {
        /// <summary>
        /// The connection.
        /// </summary>
        public NetConnection Connection { get; }

        /// <summary>
        /// NetBuffer containing any data sent on connection.
        /// </summary>
        public NetBuffer Buffer { get; }

        /// <summary>
        /// Creates a ConnectionEventArgs with a specified connection and data buffer.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="buffer">The data buffer.</param>
        public ConnectionEventArgs(NetConnection connection, NetBuffer buffer)
        {
            Connection = connection;
            Buffer = buffer;
        }
    }

    /// <summary>
    /// Event handler for receiving a packet.
    /// </summary>
    /// <param name="sender">The object which raised the event.</param>
    /// <param name="args">The event arguments.</param>
    public delegate void PacketRecievedEventHandler(object sender, PacketRecievedEventArgs args);

    /// <summary>
    /// Stores all argument data for the packet received event.
    /// </summary>
    public class PacketRecievedEventArgs : EventArgs
    {
        /// <summary>
        /// The sender of the packet.
        /// </summary>
        public NetConnection SenderConnection { get; }

        /// <summary>
        /// The packet data buffer.
        /// </summary>
        public NetBuffer Buffer { get; }

        /// <summary>
        /// Creates a PacketReceivedEventArgs with a specified sender connection and data buffer.
        /// </summary>
        /// <param name="senderConnection">The sender connection.</param>
        /// <param name="buffer">The packet data buffer.</param>
        public PacketRecievedEventArgs(NetConnection senderConnection, NetBuffer buffer)
        {
            SenderConnection = senderConnection;
            Buffer = buffer;
        }
    }

    /// <summary>
    /// Base class for all network peers.
    /// </summary>
    /// <typeparam name="T">The specific NetPeer which this peer should use.</typeparam>
    public abstract class Peer<T> where T : NetPeer
    {
        /// <summary>
        /// Event handler for any incoming messages that are handled by the peer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected delegate void IncomingMessageEventHandler(object sender, IncomingMessageEventArgs args);

        /// <summary>
        /// Stores all argument data for the incoming message event.
        /// </summary>
        protected class IncomingMessageEventArgs : EventArgs
        {
            /// <summary>
            /// The incoming message to handle.
            /// </summary>
            public NetIncomingMessage Message { get; }

            /// <summary>
            /// Creates an IncomingMessageEventArgs with a specified incoming message.
            /// </summary>
            /// <param name="message"></param>
            public IncomingMessageEventArgs(NetIncomingMessage message)
            {
                Message = message;
            }
        }

        /// <summary>
        /// Event for when a peer connects to this peer.
        /// </summary>
        public event ConnectionEventHandler PeerConnected;
        protected void OnConnected(ConnectionEventArgs args)
        {
            PeerConnected?.Invoke(this, args);
        }

        /// <summary>
        /// Event for when a peer disconnects to this peer.
        /// </summary>
        public event ConnectionEventHandler PeerDisconnected;
        private void OnDisconnected(ConnectionEventArgs args)
        {
            PeerDisconnected?.Invoke(this, args);
        }

        /// <summary>
        /// This peers network configuration.
        /// </summary>
        public NetPeerConfiguration NetConfiguration { get; private set; }

        /// <summary>
        /// Collection of packet handlers for this peer.
        /// </summary>
        public PacketHandlerCollection Packets { get; }

        /// <summary>
        /// The local ip address of the machine which this peer is running on.
        /// </summary>
        public IPAddress LocalIpAddress => NetPeer.Configuration.LocalAddress;

        /// <summary>
        /// The port of this peer.
        /// </summary>
        public int? Port => NetPeer.Configuration.Port;

        /// <summary>
        /// The network peer.
        /// </summary>
        protected T NetPeer { get; private set; }

        /// <summary>
        /// Collection of handlers for incoming messages.
        /// </summary>
        private readonly Dictionary<NetIncomingMessageType, IncomingMessageEventHandler> incomingMessageHandlers;

        protected Peer()
        {
            incomingMessageHandlers = new Dictionary<NetIncomingMessageType, IncomingMessageEventHandler>();
            Packets = new PacketHandlerCollection();
        }

        /// <summary>
        /// Listens and handles network messages.
        /// </summary>
        protected void Listen()
        {
            // Make sure that our peer is constructed.
            Validate();
            
            NetIncomingMessage message = NetPeer.ReadMessage();
            if (message == null) return;

            switch (message.MessageType)
            {
                case NetIncomingMessageType.Data:
                    int packetHeader = message.ReadInt32();
                    if (Packets.Contains(packetHeader))
                    {
                        Packets[packetHeader]?.Invoke(this, new PacketRecievedEventArgs(message.SenderConnection, message));
                    }

                    break;
                case NetIncomingMessageType.StatusChanged:
                    switch (message.SenderConnection.Status)
                    {
                        case NetConnectionStatus.Connected:
                            OnConnected(new ConnectionEventArgs(message.SenderConnection, message));
                            break;
                        case NetConnectionStatus.Disconnected:
                            OnDisconnected(new ConnectionEventArgs(message.SenderConnection, message));
                            break;
                    }
                    break;
                default:
                    // If we can handle this message type then DO IT!
                    if (incomingMessageHandlers.ContainsKey(message.MessageType))
                    {
                        incomingMessageHandlers[message.MessageType](this, new IncomingMessageEventArgs(message));
                    }
                    break;
            }

            NetPeer.Recycle(message);
        }

        /// <summary>
        /// Create a packet with a specified header type.
        /// </summary>
        /// <param name="packetType">The packet type.</param>
        /// <returns>The packet buffer.</returns>
        public NetBuffer CreatePacket(ClientOutgoingPacketType packetType) => CreatePacket((int)packetType);


        /// <summary>
        /// Create a packet with a specified header type.
        /// </summary>
        /// <param name="packetType">The packet type.</param>
        /// <returns>The packet buffer.</returns>
        public NetBuffer CreatePacket(ServerOutgoingPacketType packetType) => CreatePacket((int)packetType);


        /// <summary>
        /// Create a packet with a specified header type.
        /// </summary>
        /// <param name="packetType">The packet type.</param>
        /// <returns>The packet buffer.</returns>
        public NetBuffer CreatePacket(int packetType) => CreateMessageWithHeader(packetType);

        /// <summary>
        /// Create an outgoing message with a header.
        /// </summary>
        /// <param name="packetType">The packet header type.</param>
        /// <returns>The outgoing message with the header written into it.</returns>
        protected NetOutgoingMessage CreateMessageWithHeader(int packetType)
        {
            NetOutgoingMessage message = NetPeer.CreateMessage();
            message.Write(packetType);

            return message;
        }

        /// <summary>
        /// Creates the peer.
        /// </summary>
        /// <returns>The constructed peer.</returns>
        protected abstract T ConstructPeer();

        /// <summary>
        /// Creates the network configuration.
        /// </summary>
        /// <returns>The constructed network configuration.</returns>
        protected abstract NetPeerConfiguration ConstructNetPeerConfiguration();

        /// <summary>
        /// Handle a specific incoming message type.
        /// </summary>
        /// <param name="type">The incoming message type to handle.</param>
        /// <param name="handler">The type handler.</param>
        protected void HandleMessageType(NetIncomingMessageType type, IncomingMessageEventHandler handler)
        {
            // NetIncomingMessageType.Data is reserved
            if (type == NetIncomingMessageType.Data)
            {
                Logger.Log("Peer::HandleMessageType: \"NetIncomingMessageType.Data\" is a reserved message type!");
                return;
            }

            if (!incomingMessageHandlers.ContainsKey(type))
            {
                incomingMessageHandlers[type] = null;
            }

            incomingMessageHandlers[type] += handler;
        }

        /// <summary>
        /// Makes sure that the NetPeer is constructed.
        /// </summary>
        protected void Validate()
        {
            if (NetConfiguration == null)
            {
                NetConfiguration = ConstructNetPeerConfiguration();
            }

            if (NetPeer == null)
            {
                NetPeer = ConstructPeer();
            }
        }
    }
}
