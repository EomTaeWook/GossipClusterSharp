namespace GossipClusterSharp.Gossip.Interfaces
{
    public interface IGossipTransport
    {
        Task SendMessageAsync(GossipMessage message, string targetNodeId);

        Task StartListeningAsync();

        internal event GossipMessageHandler MessageReceived;
    }
}
