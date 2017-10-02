/*
 * Author: Shon Verch
 * File Name: Client.cs
 * Project: NetworkCryptography
 * Creation Date: 9/25/2017
 * Modified Date: 9/27/2017
 * Description: The client peer; handles all client-side networking.
 */

using Lidgren.Network;
using NetworkCryptography.Core.Networking;

namespace NetworkCryptography.Client
{
    /// <summary>
    /// The client peer; handles all client-side networking.
    /// </summary>
    public class Client : Peer<NetClient>
    {
        /// <summary>
        /// Creates the appropriate NetPeer for the client.
        /// </summary>
        /// <returns></returns>
        protected override NetClient ConstructPeer()
        {
            return new NetClient(NetConfiguration);
        }

        /// <summary>
        /// Creates the appropriate NetPeerConfiguration for the client.
        /// </summary>
        /// <returns></returns>
        protected override NetPeerConfiguration ConstructNetPeerConfiguration()
        {
            return new NetPeerConfiguration("airballoon");
        }

        /// <summary>
        /// Connect to an <value>ip address</value> on a specified <value>port</value>.
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void Connect(string ip, int port)
        {
            Validate();

            NetPeer.Start();
            NetPeer.Connect(ip, port);
        }

        /// <summary>
        /// Disconnect from the server which the client is connected to.
        /// </summary>
        public void Disconnect(string hailMessage = "")
        {
            NetPeer.Disconnect(hailMessage);
        }

        /// <summary>
        /// Send a NetBuffer of data with a specified delivery method.
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="deliveryMethod"></param>
        public void Send(NetBuffer packet, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.Unreliable) =>
            NetPeer.SendMessage((NetOutgoingMessage)packet, deliveryMethod);

        /// <summary>
        /// Send a packet header with a specified delivery method.
        /// </summary>
        /// <param name="packetHeader"></param>
        /// <param name="deliveryMethod"></param>
        public void Send(ClientOutgoingPacketType packetHeader, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.Unreliable) =>
            NetPeer.SendMessage(CreateMessageWithHeader((int)packetHeader), deliveryMethod);

        /// <summary>
        /// Runs continuous logic for the client.
        /// </summary>
        public void Tick()
        {
            Listen();
        }
    }
}
