/* Author: Shon Verch
 * File Name: ChatMessage.cs
 * Project Name: NetworkCryptography
 * Creation Date: 10/20/2017
 * Modified Date: 10/20/2017
 * Description: Represents a message sent by a user.
 */

using System;

namespace NetworkCryptography.Core
{
    /// <summary>
    /// Represents a message sent by a user.
    /// </summary>
    public class ChatMessage
    {
        /// <summary>
        /// The user who sent the message.
        /// </summary>
        public User User { get; }

        /// <summary>
        /// The message that was sent.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// The time at which the message was sent.
        /// </summary>
        public DateTime Time { get; }

        /// <summary>
        /// Initializes a <see cref="ChatMessage"/> with the specified user, message, and time.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <param name="time"></param>
        public ChatMessage(User user, string message, DateTime time)
        {
            User = user;
            Message = message;
            Time = time;
        }
    }
}
