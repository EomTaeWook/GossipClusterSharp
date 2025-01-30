namespace GossipClusterSharp.Gossip.Interfaces
{
    public interface IGossipTransport
    {
        Task SendMessageAsync(GossipMessage message, string targetEndPoint);

        Task StartListeningAsync();

        public event GossipMessageHandler MessageReceived;
    }
}
