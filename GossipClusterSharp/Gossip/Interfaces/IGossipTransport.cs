using GossipClusterSharp.Networks;

namespace GossipClusterSharp.Gossip.Interfaces
{
    public interface IGossipTransport
    {
        Task SendMessageAsync(Packet packet, string targetEndPoint);

        Task StartListeningAsync();

        public event GossipMessageHandler MessageReceived;
    }
}
