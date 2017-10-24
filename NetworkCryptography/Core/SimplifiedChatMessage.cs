/*
 * Author: Shon Verch
 * File Name: SimplifiedChatMessage.cs
 * Project Name: NetworkCryptography
 * Creation Date: 10/20/2017
 * Modified Date: 10/20/2017
 * Description: A chat message which only stores essential data; which can be used to reconsruct the message.
 */
namespace NetworkCryptography.Core
{
    /// <summary>
    /// A chat message which only stores essential data; which can be used to reconsruct the message.
    /// </summary>
    public sealed class SimplifiedChatMessage
    {
        /// <summary>
        /// The id of the user who sent the message.
        /// </summary>
        public int UserId { get; }

        /// <summary>
        /// The message.
        /// </summary>
        public byte[] Message { get; }

        /// <summary>
        /// The time the message was sent in a binary format.
        /// </summary>
        public long TimeInBinary { get; }

        /// <summary>
        /// Initializes a <see cref="SimplifiedChatMessage"/> with the specified id, messsage, and time.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="message"></param>
        /// <param name="timeInBinary"></param>
        public SimplifiedChatMessage(int userId, byte[] message, long timeInBinary)
        {
            UserId = userId;
            Message = message;
            TimeInBinary = timeInBinary;
        }
    }
}
