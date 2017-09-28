using Lidgren.Network;
using NetworkCryptography.Core.Networking;

namespace NetworkCryptography.Client
{
    public class Client : Peer<NetClient>
    {
        protected override NetClient ConstructPeer()
        {
            return new NetClient(NetConfiguration);
        }

        protected override NetPeerConfiguration ConstructNetPeerConfiguration()
        {
            return new NetPeerConfiguration("airballoon");
        }

        public void Connect(string ip, int port)
        {
            Validate();
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

        public void Tick()
        {
            Listen();
        }
    }
}
