using GossipClusterSharp.Exceptions;
using GossipClusterSharp.Gossip;
using GossipClusterSharp.Internals;

namespace GossipClusterSharp.Cluster
{
    public class NodeRegistry : INodeRegistry
    {
        private readonly Dictionary<string, NodeState> _nodeStates = new();

        public void RegisterNode(NodeState node)
        {
            if (_nodeStates.TryAdd(node.NodeId, node) == false)
            {
                throw new NodeAlreadyExistsException(node.NodeId);
            }
        }

        public void RemoveNode(string nodeId)
        {
            _nodeStates.Remove(nodeId);
        }

        public IEnumerable<NodeState> GetAllNodeStates()
        {
            return _nodeStates.Values;
        }

        public NodeState GetNodeState(string nodeId)
        {
            _nodeStates.TryGetValue(nodeId, out NodeState node);

            return node;
        }
        public List<NodeState> GetRandomNode(string localNodeId, int count)
        {
            if (_nodeStates.Count == 0)
            {
                return null;
            }

            var availableCount = Math.Max(2, count);
            List<NodeState> nodes = new(_nodeStates.Values.Where(r => r.NodeId != localNodeId));
            FisherYatesShuffle.Shuffle(nodes);
            return nodes.Take(availableCount).ToList();
        }
        public int GetAliveNodeCount()
        {
            return _nodeStates.Values.Count(n => n.IsAlive);
        }
    }
}
