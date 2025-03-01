namespace GossipClusterSharp.Gossip.Interfaces
{
    public interface IGossipService
    {
        GossipNode GetLocalNode();
        Task StartAsync();

        Task StopAsync();
    }
}
