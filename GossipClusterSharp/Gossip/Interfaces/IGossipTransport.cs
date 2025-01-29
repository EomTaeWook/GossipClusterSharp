namespace GossipClusterSharp.Gossip.Interfaces
{
    public interface IGossipTransport
    {
        Task SendMessageAsync(GossipMessage message, string targetNodeId);

        Task StartListeningAsync();

        public event GossipMessageHandler MessageReceived;
    }
}
