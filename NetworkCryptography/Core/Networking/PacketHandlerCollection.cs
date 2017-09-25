using System.Collections.Generic;

namespace NetworkCryptography.Core.Networking
{
    public sealed class PacketHandlerCollection
    {
        public int Count => handlers.Count;

        public PacketRecievedEventHandler this[ClientOutgoingPacketType packetType]
        {
            get => this[(int)packetType];
            set => this[(int) packetType] = value;
        }

        public PacketRecievedEventHandler this[ServerOutgoingPacketType packetType]
        {
            get => this[(int)packetType];
            set => this[(int)packetType] = value;
        }
  
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

        public bool Contains(ClientOutgoingPacketType packetType) => Contains((int) packetType); 
        public bool Contains(ServerOutgoingPacketType packetType) => Contains((int) packetType);

        public bool Contains(int header)
        {
            return handlers.ContainsKey(header);
        }
    }
}
