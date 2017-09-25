using Lidgren.Network;
using NetworkCryptography.Core.Networking;

namespace NetworkCryptography.App
{
    public class Client : Peer<NetClient>, IAppProvider
    {
        public override NetPeerConfiguration NetConfiguration { get; } = new NetPeerConfiguration("chat-app");

        protected override NetClient ConstructPeer()
        {
            return new NetClient(NetConfiguration);
        }

        public void Connect(string ip, int port)
        {
            NetPeer.Start();
            NetPeer.Connect(ip, port);
        }

        public void Disconnect()
        {
            NetPeer.Disconnect(string.Empty);
        }

        public void Send(NetBuffer packet, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.Unreliable) =>
            NetPeer.SendMessage((NetOutgoingMessage)packet, deliveryMethod);

        public void Send(ClientOutgoingPacketType packetHeader, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.Unreliable) =>
            NetPeer.SendMessage(CreateMessageWithHeader((int)packetHeader), deliveryMethod);

        public void Initialize()
        {
            Validate();
        }

        public void Tick()
        {
            Listen();
        }
    }
}
