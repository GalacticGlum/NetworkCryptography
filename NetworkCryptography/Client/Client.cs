/*
 * Author: Shon Verch
 * File Name: Client.cs
 * Project: NetworkCryptography
 * Creation Date: 9/25/2017
 * Modified Date: 10/17/2017
 * Description: The client peer; handles all client-side networking.
 */

using Lidgren.Network;
using NetworkCryptography.Core.Logging;
using NetworkCryptography.Core.Networking;

namespace NetworkCryptography.Client
{
    /// <summary>
    /// The client peer; handles all client-side networking.
    /// </summary>
    public sealed class Client : Peer<NetClient>
    {
        /// <summary>
        /// Manager for all users connected.
        /// </summary>
        public ClientUserManager UserManager { get; }

        /// <summary>
        /// Manager for all chat messages.
        /// </summary>
        public ClientChatMessageManager ChatMessageManager { get; }

        public Client()
        {
            UserManager = new ClientUserManager(Packets);
            ChatMessageManager = new ClientChatMessageManager(Packets);

            Packets[ServerOutgoingPacketType.Pong] += HandlePongMessage;
        }

        /// <summary>
        /// Handle a ping response by logging pong.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void HandlePongMessage(object sender, PacketRecievedEventArgs args)
        {
            Logger.Log("Pong");
        }

        /// <summary>
        /// Send a ping request to the server.
        /// </summary>
        public void Ping()
        {
            Send(ClientOutgoingPacketType.Ping, NetDeliveryMethod.ReliableOrdered);
        }

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
        /// <param name="username"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void Connect(string username, string ip, int port)
        {
            Validate();
            NetPeer.Start();
            UserManager.Clear();

            NetPeer.Connect(ip, port, NetPeer.CreateMessage(username));
        }

        /// <summary>
        /// Disconnect from the server which the client is connected to.
        /// </summary>
        public void Disconnect()
        {
            if (NetPeer == null || NetPeer.ConnectionStatus != NetConnectionStatus.Connected) return;

            // Send id with disconnection so we can identify who disconnected.
            NetPeer.Disconnect(UserManager.BelongingUser.Id.ToString());
        }

        /// <summary>
        /// Send a NetBuffer of dataBuffer with a specified delivery method.
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
