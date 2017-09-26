using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Net;
using System.Web;
using NetworkCryptography.Core.Logging;
using Lidgren.Network;

namespace NetworkCryptography.Core.Networking
{
    public delegate void ConnectionEventHandler(object sender, ConnectionEventArgs args);
    public class ConnectionEventArgs : EventArgs
    {
        public NetConnection Connection { get; }
        public NetBuffer Buffer { get; }

        public ConnectionEventArgs(NetConnection connection, NetBuffer buffer)
        {
            Connection = connection;
            Buffer = buffer;
        }
    }

    public delegate void PacketRecievedEventHandler(object sender, PacketRecievedEventArgs args);
    public class PacketRecievedEventArgs : EventArgs
    {
        public NetConnection SenderConnection { get; }
        public NetBuffer Buffer { get; }
        public PacketRecievedEventArgs(NetConnection senderConnection, NetBuffer buffer)
        {
            SenderConnection = senderConnection;
            Buffer = buffer;
        }
    }

    public abstract class Peer<T> where T : NetPeer
    {
        protected delegate void IncomingMessageEventHandler(object sender, IncomingMessageEventArgs args);
        protected class IncomingMessageEventArgs : EventArgs
        {
            public NetIncomingMessage Message { get; }
            public IncomingMessageEventArgs(NetIncomingMessage message)
            {
                Message = message;
            }
        }

        public event ConnectionEventHandler PeerConnected;
        protected void OnConnected(ConnectionEventArgs args)
        {
            PeerConnected?.Invoke(this, args);
        }

        public event ConnectionEventHandler PeerDisconnected;
        private void OnDisconnected(ConnectionEventArgs args)
        {
            PeerDisconnected?.Invoke(this, args);
        }

        public NetPeerConfiguration NetConfiguration { get; private set; }
        public PacketHandlerCollection Packets { get; }

        public IPAddress LocalIpAddress => NetPeer.Configuration.LocalAddress;
        public int? Port => NetPeer.Configuration.Port;

        protected T NetPeer { get; private set; }

        private readonly Dictionary<NetIncomingMessageType, IncomingMessageEventHandler> incomingMessageHandlers;

        protected Peer()
        {
            incomingMessageHandlers = new Dictionary<NetIncomingMessageType, IncomingMessageEventHandler>();
            Packets = new PacketHandlerCollection();
        }

        protected void Listen()
        {
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
                    if (incomingMessageHandlers.ContainsKey(message.MessageType))
                    {
                        incomingMessageHandlers[message.MessageType](this, new IncomingMessageEventArgs(message));
                    }
                    break;
            }

            NetPeer.Recycle(message);
        }

        public NetBuffer CreatePacket(ClientOutgoingPacketType packetType) => CreatePacket((int)packetType);
        public NetBuffer CreatePacket(ServerOutgoingPacketType packetType) => CreatePacket((int)packetType);
        public NetBuffer CreatePacket(int packetType) => CreateMessageWithHeader(packetType);

        protected NetOutgoingMessage CreateMessageWithHeader(int packetType)
        {
            NetOutgoingMessage message = NetPeer.CreateMessage();
            message.Write(packetType);

            return message;
        }


        protected abstract T ConstructPeer();
        protected abstract NetPeerConfiguration ConstructNetPeerConfiguration();

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
