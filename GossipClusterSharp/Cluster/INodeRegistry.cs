using GossipClusterSharp.Gossip;

namespace GossipClusterSharp.Cluster
{
    public interface INodeRegistry
    {
        void RegisterNode(NodeState node);
        void RemoveNode(string nodeId);
        IEnumerable<NodeState> GetAllNodeStates();
        NodeState GetNodeState(string nodeId);
        List<NodeState> GetRandomNode();
        int GetAliveNodeCount();
    }
}
