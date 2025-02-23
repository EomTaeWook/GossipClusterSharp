using GossipClusterSharp.Gossip;

namespace GossipClusterSharp.Cluster
{
    public interface INodeRegistry
    {
        void RegisterNode(GossipNode node);
        void RemoveNode(string nodeId);
        IEnumerable<GossipNode> GetAllNodes();
        GossipNode GetNode(string nodeId);
        IEnumerable<GossipNode> GetRandomNode(int count);
        int GetAliveNodeCount();
    }
}
