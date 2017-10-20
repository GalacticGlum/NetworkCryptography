/*
 * Author: Shon Verch
 * File Name: ChatCommand.cs
 * Project Name: NetworkCryptography
 * Creation Date: 10/20/2017
 * Modified Date: 10/20/2017
 * Description: A chat command.
 */

namespace NetworkCryptography.Core.ChatCommands
{
    /// <summary>
    /// A chat command.
    /// </summary>
    public abstract class ChatCommand
    {
        /// <summary>
        /// The name of the command.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <param name="chatMessage">The message data.</param>
        /// <param name="execute">Indicates whether the command should actually do something.</param>
        /// <returns>The chat message after execution.</returns>
        public abstract ChatMessage Execute(ChatMessage chatMessage, bool execute);
    }
}
