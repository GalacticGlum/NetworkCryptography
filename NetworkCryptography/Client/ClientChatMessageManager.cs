/*
 * Author: Shon Verch
 * File Name: ClientChatMessageManager.cs
 * Project Name: NetworkCryptography
 * Creation Date: 10/17/2017
 * Modified Date: 10/20/2017
 * Description: Manages all chat messages that have been sent to/form this client.
 */

using System;
using Lidgren.Network;
using NetworkCryptography.Core;
using NetworkCryptography.Core.ChatCommands;
using NetworkCryptography.Core.Logging;
using NetworkCryptography.Core.Networking;

namespace NetworkCryptography.Client
{
    /// <summary>
    /// Event for whenever the client receives a <see cref="ChatMessage"/>.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name=""></param>
    public delegate void ChatMessageReceivedEventHandler(object sender, ChatMessageEventArgs args);

    /// <summary>
    /// Generic event arguments for any chat message event.
    /// </summary>
    public sealed class ChatMessageEventArgs : EventArgs
    {
        /// <summary>
        /// The chat message sent.
        /// </summary>
        public ChatMessage ChatMessage { get; }

        /// <summary>
        /// Initializes the <see cref="ChatMessageEventArgs"/> with a specified <paramref name="chatMessage"/>.
        /// </summary>
        /// <param name="chatMessage">The chat message sent.</param>
        public ChatMessageEventArgs(ChatMessage chatMessage)
        {
            ChatMessage = chatMessage;
        }
    }

    /// <summary>
    /// Manages all chat messages that have been sent to/form this client.
    /// </summary>
    public sealed class ClientChatMessageManager
    {
        /// <summary>
        /// Event which is raised whenever we receive a chat message.
        /// </summary>
        public event ChatMessageReceivedEventHandler ChatMessageReceived;

        /// <summary>
        /// Raises the ChatMessageReceived event.
        /// </summary>
        /// <param name="chatMessage"></param>
        /// <param name="processForCommand"></param>
        private void OnChatMessageReceived(ChatMessage chatMessage, bool processForCommand = true)
        {
            // Process the chat message for any commands.
            // Since we still need to process the command text we run the processor but don't actually execute the actions.
            ChatMessage processedChatMessage = ChatCommandProcessor.Process(chatMessage, processForCommand);          
            ChatMessageReceived?.Invoke(this, new ChatMessageEventArgs(processedChatMessage));
        }

        /// <summary>
        /// Initializes a <see cref="ClientChatMessageManager"/> and all events.
        /// </summary>
        /// <param name="packetHandler"></param>
        public ClientChatMessageManager(PacketHandlerCollection packetHandler)
        {
            packetHandler[ServerOutgoingPacketType.RelayMessage] += HandleRelayMessage;
            packetHandler[ServerOutgoingPacketType.SendMessageHistory] += HandleMessageHistory;
        }

        /// <summary>
        /// Send a message to the server.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public void SendMessage(string message)
        {
            DateTime messageTime = DateTime.Now;

            // Encrypt our message
            byte[] encryptedMessage = CoreClientApp.CryptographicEngine.Encrypt(message);
            SimplifiedChatMessage encryptedChatMessage = new SimplifiedChatMessage(CoreClientApp.Client.UserManager.BelongingUser.Id, encryptedMessage, messageTime.ToBinary());

            NetBuffer messageBuffer = CoreClientApp.Client.CreatePacket(ClientOutgoingPacketType.SendMessage);
            messageBuffer.Write(encryptedChatMessage.UserId);
            messageBuffer.Write(encryptedChatMessage.Message.Length);
            messageBuffer.Write(encryptedChatMessage.Message);
            messageBuffer.Write(encryptedChatMessage.TimeInBinary);
            CoreClientApp.Client.Send(messageBuffer, NetDeliveryMethod.ReliableOrdered);

            /*
             * This is a slight "hack"/cheat:
             *      since our ChatroomPageDataContext listens to this event (and updates it's messages list based on it) 
             *      we can locally display the message through this event.
             *      
             * Since this is raising the event locally, there is no reason to encrypt our message so let's send the original.
             */          
            OnChatMessageReceived(new ChatMessage(CoreClientApp.Client.UserManager.BelongingUser, message, messageTime));
        }

        /// <summary>
        /// Handles a message relay from the server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void HandleRelayMessage(object sender, PacketRecievedEventArgs args)
        {
            SimplifiedChatMessage message = ReadMessageFromNetwork(args.Message);

            // If the user id sent is our user id then there is no reason to continue as we have already handled this message locally.
            if (message.UserId == CoreClientApp.Client.UserManager.BelongingUser.Id) return;

            if (!TryParseFromSimplified(message, out ChatMessage chatMessage)) return;
            OnChatMessageReceived(chatMessage);
        }

        /// <summary>
        /// Handles the message history sent from the server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void HandleMessageHistory(object sender, PacketRecievedEventArgs args)
        {
            int count = args.Message.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                SimplifiedChatMessage message = ReadMessageFromNetwork(args.Message);
                if(!TryParseFromSimplified(message, out ChatMessage chatMessage)) continue;
                OnChatMessageReceived(chatMessage, false);
            }
        }

        /// <summary>
        /// Reads a <see cref="SimplifiedChatMessage"/> from a <see cref="NetBuffer"/>.
        /// </summary>
        /// <param name="buffer">The <see cref="NetBuffer"/> to read from.</param>
        /// <returns>The read-in <see cref="SimplifiedChatMessage"/>.</returns>
        private static SimplifiedChatMessage ReadMessageFromNetwork(NetBuffer buffer)
        {
            int userId = buffer.ReadInt32();
            int messageLength = buffer.ReadInt32();
            byte[] message = buffer.ReadBytes(messageLength);
            long time = buffer.ReadInt64();

            return new SimplifiedChatMessage(userId, message, time);
        }

        /// <summary>
        /// Converts a <paramref name="simplifiedChatMessage"/> to a <see cref="ChatMessage"/>. A return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="simplifiedChatMessage">The <see cref="SimplifiedChatMessage"/> to convert.</param>
        /// <param name="chatMessage">The <see cref="ChatMessage"/> to store the result of the conversion in.</param>
        private static bool TryParseFromSimplified(SimplifiedChatMessage simplifiedChatMessage, out ChatMessage chatMessage)
        {
            chatMessage = null;
            if (simplifiedChatMessage.Message.Length <= 0) return false;

            User user = CoreClientApp.Client.UserManager[simplifiedChatMessage.UserId];
            if (user == null) return false;

            DateTime time = DateTime.FromBinary(simplifiedChatMessage.TimeInBinary);
            chatMessage = new ChatMessage(user, CoreClientApp.CryptographicEngine.Decrypt(simplifiedChatMessage.Message), time);

            return true;
        }
    }
}
