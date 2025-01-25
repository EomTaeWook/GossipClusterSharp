namespace GossipClusterSharp.Gossip.Interfaces
{
    public delegate void GossipMessageHandler(GossipMessage message);
    public interface IGossipTransport
    {
        Task SendMessageAsync(GossipMessage message, string targetNodeId);

        Task StartListeningAsync();

        event GossipMessageHandler MessageReceived;


    }
}
