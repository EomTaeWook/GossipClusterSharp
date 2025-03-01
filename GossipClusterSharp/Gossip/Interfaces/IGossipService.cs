namespace GossipClusterSharp.Gossip.Interfaces
{
    public interface IGossipService
    {
        Task StartAsync();

        Task StopAsync();
    }
}
