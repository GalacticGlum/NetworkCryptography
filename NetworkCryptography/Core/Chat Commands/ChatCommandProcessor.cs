/*
 * Author: Shon Verch
 * File Name: ChatCommandProcessor.cs
 * Project Name: NetworkCryptography
 * Creation Date: 10/20/2017
 * Modified Date: 10/20/2017
 * Description: Processes and executes chat commands.
 */

using System;
using System.Collections.Generic;
using System.Reflection;

namespace NetworkCryptography.Core.ChatCommands
{
    /// <summary>
    /// Attribute flags that a chat command should be used at runtime.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class UseChatCommandAttribute : Attribute
    {
    }

    /// <summary>
    /// Processes and executes chat commands.
    /// </summary>
    public static class ChatCommandProcessor
    {
        /// <summary>
        /// <see cref="ChatCommand"/> lookup table.
        /// </summary>
        private static readonly Dictionary<string, ChatCommand> commands;

        /// <summary>
        /// Initializes the <see cref="ChatCommandProcessor"/>.
        /// </summary>
        static ChatCommandProcessor()
        {
            commands = new Dictionary<string, ChatCommand>();
            InitializeCommands();
        }

        /// <summary>
        /// Initialize all chat commands.
        /// </summary>
        private static void InitializeCommands()
        {
            Type useChatCommandAttributeType = typeof(UseChatCommandAttribute);
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if(!Attribute.IsDefined(type, useChatCommandAttributeType)) continue;
                    Add(type);
                }
            }
        }

        /// <summary>
        /// Process a <see cref="ChatMessage"/>.
        /// </summary>
        /// <param name="message">The <see cref="ChatMessage"/> to process.</param>
        public static ChatMessage Process(ChatMessage message, bool executeActions)
        {
            if (string.IsNullOrEmpty(message?.Message)) return message;
            string commandName = message.Message.Split(' ')[0];

            if (!commands.ContainsKey(commandName)) return message;

            string messageWithoutCommandName = message.Message.Substring(commandName.Length).Trim();
            ChatMessage newMessage = new ChatMessage(message.User, messageWithoutCommandName, message.Time);
            return commands[commandName].Execute(newMessage, executeActions);
        }

        /// <summary>
        /// Adds a <see cref="ChatCommand"/> of <paramref name="type"/> to the processor.
        /// </summary>
        /// <param name="type">The type of the command.</param>
        private static void Add(Type type)
        {
            if (!type.IsSubclassOf(typeof(ChatCommand))) return;

            ChatCommand command = (ChatCommand) Activator.CreateInstance(type);
            commands.Add(command.Name, command);
        }
    }
}
