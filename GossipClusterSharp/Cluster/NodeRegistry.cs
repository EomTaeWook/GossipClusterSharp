using GossipClusterSharp.Exceptions;
using GossipClusterSharp.Gossip.Interfaces;
using GossipClusterSharp.Internals;

namespace GossipClusterSharp.Cluster
{
    public class NodeRegistry : INodeRegistry
    {
        private readonly Dictionary<string, INodeState> _nodeStates = new();

        public void RegisterNode(INodeState node)
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

        public IEnumerable<INodeState> GetAllNodeStates()
        {
            return _nodeStates.Values;
        }

        public INodeState GetNodeState(string nodeId)
        {
            _nodeStates.TryGetValue(nodeId, out INodeState node);

            return node;
        }
        public List<INodeState> GetRandomNode()
        {
            if (_nodeStates.Count == 0)
            {
                return null;
            }

            var availableCount = Math.Min(2, _nodeStates.Count);
            List<INodeState> nodes = new(_nodeStates.Values);
            FisherYatesShuffle.Shuffle(nodes);
            return nodes.Take(availableCount).ToList();
        }
        public int GetAliveNodeCount()
        {
            return _nodeStates.Values.Count(n => n.IsAlive);
        }
        public void UpdateNodeState(string nodeId, bool isAlive)
        {
            if (_nodeStates.TryGetValue(nodeId, out var node) == false)
            {
                throw new NodeNotFoundException(nodeId);
            }
            node.IsAlive = isAlive;
        }
    }
}
