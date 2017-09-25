using Lidgren.Network;

namespace NetworkCryptography.Core.Networking
{
    // ReSharper disable once UnusedTypeParameter
    public interface INetworked<T> where T : new()
    {
        void Serialize(NetBuffer packet);
        void Deserialize(NetBuffer packet);
    }
}
