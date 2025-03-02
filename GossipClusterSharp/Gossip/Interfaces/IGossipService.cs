namespace GossipClusterSharp.Gossip.Interfaces
{
    public interface IGossipService
    {
        GossipNode GetLocalNode();
        Task SendAsync(GossipMessage message, GossipNode targetNode);
        Task StartAsync();
        Task StopAsync();
    }
}
