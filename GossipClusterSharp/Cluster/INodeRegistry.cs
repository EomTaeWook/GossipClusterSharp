using GossipClusterSharp.Gossip.Interfaces;

namespace GossipClusterSharp.Cluster
{
    public interface INodeRegistry
    {
        void RegisterNode(INodeState node);
        void RemoveNode(string nodeId);
        IEnumerable<INodeState> GetAllNodeStates();
        INodeState GetNodeState(string nodeId);
        List<INodeState> GetRandomNode();
        int GetAliveNodeCount();
        void UpdateNodeState(string nodeId, bool isAlive);
    }
}
