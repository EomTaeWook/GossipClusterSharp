using GossipClusterSharp.Exceptions;
using GossipClusterSharp.Gossip.Interfaces;

namespace GossipClusterSharp.Cluster
{
    internal class NodeRegistry
    {
        private readonly Dictionary<string, INodeState> _nodeStates = new();

        public void RegisterNode(INodeState node)
        {
            if (_nodeStates.ContainsKey(node.NodeId))
            {
                throw new NodeAlreadyExistsException(node.NodeId);
            }
            _nodeStates[node.NodeId] = node;
        }

        public void RemoveNode(string nodeId)
        {
            if (!_nodeStates.Remove(nodeId))
            {
                throw new NodeNotFoundException(nodeId);
            }
        }

        public IEnumerable<INodeState> GetAllNodeStates()
        {
            return _nodeStates.Values;
        }

        public INodeState GetNodeState(string nodeId)
        {
            if (_nodeStates.TryGetValue(nodeId, out INodeState node) == false)
            {
                throw new NodeNotFoundException(nodeId);
            }

            return node;
        }
        public INodeState GetRandomNode()
        {
            if (_nodeStates.Count == 0)
            {
                return null;
            }
            var random = new Random();
            var index = random.Next(_nodeStates.Count);
            return _nodeStates.Values.ElementAt(index);
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
