using System.Collections.Generic;
using Lidgren.Network;
using NetworkCryptography.Core.Networking;

namespace NetworkCryptography.App
{
    public class Server : Peer<NetServer>, IAppProvider
    {
        public bool IsRunning { get; private set; }

        private const int ServerPort = 7777;
        private const int MaximumConnections = 100;

        public override NetPeerConfiguration NetConfiguration { get; } = new NetPeerConfiguration("chat-app")
        {
            Port = ServerPort,
            MaximumConnections = MaximumConnections
        };

        protected override NetServer ConstructPeer()
        {
            NetConfiguration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            return new NetServer(NetConfiguration);
        }

        public void Start()
        {
            NetPeer.Start();
            IsRunning = true;
        }

        public void Stop()
        {
            NetPeer.Shutdown(string.Empty);
            IsRunning = false;
        }

        public void Send(NetBuffer packet, NetConnection connection, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.Unreliable)
        {
            NetPeer.SendMessage((NetOutgoingMessage)packet, connection, deliveryMethod);
        }

        public void Send(NetBuffer packet, IList<NetConnection> connections, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.Unreliable)
        {
            foreach (NetConnection netConnection in connections)
            {
                NetPeer.SendMessage((NetOutgoingMessage)packet, netConnection, deliveryMethod);
            }
        }

        public void Send(ServerOutgoingPacketType header, NetConnection connection, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.Unreliable)
        {
            NetPeer.SendMessage(CreateMessageWithHeader((int)header), connection, deliveryMethod);
        }

        public void Send(ServerOutgoingPacketType header, IList<NetConnection> connections, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.Unreliable)
        {
            foreach (NetConnection netConnection in connections)
            {
                NetPeer.SendMessage(CreateMessageWithHeader((int)header), netConnection, deliveryMethod);
            }
        }

        public void SendToAll(NetBuffer packet, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.Unreliable)
        {
            NetPeer.SendToAll((NetOutgoingMessage)packet, deliveryMethod);
        }

        public void Initialize()
        {
            Validate();
            HandleMessageType(NetIncomingMessageType.ConnectionApproval, (sender, args) =>
            {
                args.Message.SenderConnection.Approve();
            });
        }

        public void Tick()
        {
            Listen();
        }
    }
}
