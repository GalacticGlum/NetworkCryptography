/*
 * Author: Shon Verch
 * File Name: PacketHandlerCollection.cs
 * Project: NetworkCryptography
 * Creation Date: 9/25/2017
 * Modified Date: 9/25/2017
 * Description: Collection of packet handles, both server and client outgoing.
 */

using System.Collections.Generic;

namespace NetworkCryptography.Core.Networking
{
    /// <summary>
    /// Collection of packet handles, both server and client outgoing.
    /// </summary>
    public sealed class PacketHandlerCollection
    {
        /// <summary>
        /// The amount of handlers in the collection.
        /// </summary>
        public int Count => handlers.Count;

        /// <summary>
        /// Retrieve client outgoing handler of the specified type.
        /// </summary>
        /// <param name="packetType">The type of handler.</param>
        /// <returns>The packed handler.</returns>
        public PacketRecievedEventHandler this[ClientOutgoingPacketType packetType]
        {
            get => this[(int)packetType];
            set => this[(int) packetType] = value;
        }

        /// <summary>
        /// Retrieve server outgoing handler of the specified type.
        /// </summary>
        /// <param name="packetType">The type of handler.</param>
        /// <returns>The packed handler.</returns>
        public PacketRecievedEventHandler this[ServerOutgoingPacketType packetType]
        {
            get => this[(int)packetType];
            set => this[(int)packetType] = value;
        }

        /// <summary>
        /// Retrieve a handler with a specified packet header.
        /// </summary>
        /// <param name="header">The packet header.</param>
        /// <returns>The packed handler.</returns>
        public PacketRecievedEventHandler this[int header]
        {
            get
            {
                if (!handlers.ContainsKey(header))
                {
                    handlers[header] = null;
                }

                return handlers[header];
            }
            set
            {
                if (!handlers.ContainsKey(header))
                {
                    handlers[header] = value;
                }
                else
                {
                    handlers[header] += value;
                }
            }
        }

        private readonly Dictionary<int, PacketRecievedEventHandler> handlers;

        public PacketHandlerCollection()
        {
            handlers = new Dictionary<int, PacketRecievedEventHandler>();
        }

        /// <summary>
        /// Indicates whether a packet has a handler in the collection.
        /// </summary>
        /// <param name="packetType">The packet type.</param>
        /// <returns>A bool indicating whether or not the packet is in the collection.</returns>
        public bool Contains(ClientOutgoingPacketType packetType) => Contains((int) packetType);

        /// <summary>
        /// Indicates whether a packet has a handler in the collection.
        /// </summary>
        /// <param name="packetType">The packet type.</param>
        /// <returns>A bool indicating whether or not the packet is in the collection.</returns>
        public bool Contains(ServerOutgoingPacketType packetType) => Contains((int) packetType);


        /// <summary>
        /// Indicates whether a packet has a handler in the collection.
        /// </summary>
        /// <param name="header">The packet header.</param>
        /// <returns>A bool indicating whether or not the packet is in the collection.</returns>
        public bool Contains(int header)
        {
            return handlers.ContainsKey(header);
        }
    }
}
