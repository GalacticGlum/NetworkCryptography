/*
 * Author: Shon Verch
 * File Name: ServerChatMessageManager.cs
 * Project Name: NetworkCryptography
 * Creation Date: 10/20/2017
 * Modified Date: 10/20/2017
 * Description:  Manages all chat messages.
 */

using System.Collections;
using System.Collections.Generic;
using Lidgren.Network;
using NetworkCryptography.Core;
using NetworkCryptography.Core.DataStructures;
using NetworkCryptography.Core.Helpers;
using NetworkCryptography.Core.Logging;
using NetworkCryptography.Core.Networking;

namespace NetworkCryptography.Server
{
    /// <summary>
    /// Manages all chat messages.
    /// </summary>
    public sealed class ServerChatMessageManager : IEnumerable<SimplifiedChatMessage>
    {
        /// <summary>
        /// Retrieves a <see cref="SimplifiedChatMessage"/> at the specified index.
        /// </summary>
        /// <param name="index">The index of the message.</param>
        public SimplifiedChatMessage this[int index] => Get(index);

        /// <summary>
        /// The amount of messages in this manager.
        /// </summary>
        public int Count => messages.Count;

        /// <summary>
        /// Max amount of messages to save.
        /// </summary>
        private const int MaxMessageHistory = 1000;

        /// <summary>
        /// Message list.
        /// </summary>
        private readonly FixedList<SimplifiedChatMessage> messages;

        public ServerChatMessageManager(PacketHandlerCollection packetHandler)
        {
            messages = new FixedList<SimplifiedChatMessage>(MaxMessageHistory);        
            packetHandler[ClientOutgoingPacketType.SendMessage] += HandleNewMessage;
        }

        /// <summary>
        /// Sends all message history to a target client.
        /// </summary>
        /// <param name="target"></param>
        public void SendMessageHistory(NetConnection target)
        {
            NetBuffer messageBuffer = CoreServerApp.Server.CreatePacket(ServerOutgoingPacketType.SendMessageHistory);

            messageBuffer.Write(Count);
            foreach (SimplifiedChatMessage chatMessage in messages)
            {
                messageBuffer.Write(chatMessage.UserId);
                messageBuffer.Write(chatMessage.Message.Length);
                messageBuffer.Write(chatMessage.Message);
                messageBuffer.Write(chatMessage.TimeInBinary);
            }

            CoreServerApp.Server.Send(messageBuffer, target, NetDeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Handles a message sent from a client.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void HandleNewMessage(object sender, PacketRecievedEventArgs args)
        {
            int userId = args.Message.ReadInt32();
            int messageLength = args.Message.ReadInt32();
            byte[] message = args.Message.ReadBytes(messageLength);
            long time = args.Message.ReadInt64();

            SimplifiedChatMessage chatMessage = new SimplifiedChatMessage(userId, message, time);

            Logger.Log($"Adding message: \"{message}\"");
            Add(chatMessage);
            Relay(chatMessage);
        }

        /// <summary>
        /// Send the specified message to all clients.
        /// </summary>
        /// <param name="message">The message to relay.</param>
        private static void Relay(SimplifiedChatMessage message)
        {
            NetBuffer messageBuffer = CoreServerApp.Server.CreatePacket(ServerOutgoingPacketType.RelayMessage);
            messageBuffer.Write(message.UserId);
            messageBuffer.Write(message.Message.Length);
            messageBuffer.Write(message.Message);
            messageBuffer.Write(message.TimeInBinary);

            CoreServerApp.Server.SendToAll(messageBuffer, NetDeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Adds a new <see cref="SimplifiedChatMessage"/> to the manager database.
        /// </summary>
        /// <param name="chatMessage">The message to add.</param>
        public void Add(SimplifiedChatMessage chatMessage) => messages.Add(chatMessage);

        /// <summary>
        /// Removes a <see cref="SimplifiedChatMessage"/> with the specified index.
        /// </summary>
        /// <param name="index">The index of the message.</param>
        /// <returns>The <see cref="SimplifiedChatMessage"/> removed.</returns>
        public SimplifiedChatMessage Remove(int index)
        {
            if (index < 0 || index >= MaxMessageHistory) return null;
            SimplifiedChatMessage chatMessage = Get(index);
            messages.Remove(chatMessage);

            return chatMessage;
        }

        /// <summary>
        /// Retrieves a <see cref="SimplifiedChatMessage"/> at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public SimplifiedChatMessage Get(int index) => index < 0 || index >= MaxMessageHistory ? null : messages[index];

        /// <summary>
        /// Returns an enumerator that iterates through all the messages in this manager.
        /// </summary>
        public IEnumerator<SimplifiedChatMessage> GetEnumerator() => messages.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through all the messages in this manager.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
