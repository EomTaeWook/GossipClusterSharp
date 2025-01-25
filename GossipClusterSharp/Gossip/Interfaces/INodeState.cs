namespace GossipClusterSharp.Gossip.Interfaces
{
    public interface INodeState
    {
        string NodeId { get; }
        bool IsAlive { get; set; }
        bool IsMaster { get; set; }
        public int Priority { get; }
        public string Endpoint { get; }
    }
}
