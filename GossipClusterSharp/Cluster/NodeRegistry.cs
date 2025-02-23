using GossipClusterSharp.Exceptions;
using GossipClusterSharp.Gossip;
using GossipClusterSharp.Internals;

namespace GossipClusterSharp.Cluster
{
    public class NodeRegistry : INodeRegistry
    {
        private readonly Dictionary<string, GossipNode> _nodes = new();

        public void RegisterNode(GossipNode node)
        {
            if (_nodes.TryAdd(node.NodeId, node) == false)
            {
                throw new NodeAlreadyExistsException(node.NodeId);
            }
        }

        public void RemoveNode(string nodeId)
        {
            _nodes.Remove(nodeId);
        }

        public IEnumerable<GossipNode> GetAllNodes()
        {
            return _nodes.Values;
        }

        public GossipNode GetNode(string nodeId)
        {
            _nodes.TryGetValue(nodeId, out GossipNode node);

            return node;
        }
        public IEnumerable<GossipNode> GetRandomNode(int count)
        {
            if (_nodes.Count == 0)
            {
                return null;
            }
            var availableCount = Math.Max(2, count);
            List<GossipNode> nodes = new(_nodes.Count);
            foreach (var node in _nodes.Values)
            {
                nodes.Add(node);
            }
            FisherYatesShuffle.Shuffle(nodes);
            return nodes.Take(availableCount);
        }
        public int GetAliveNodeCount()
        {
            return _nodes.Values.Count(n => n.IsAlive);
        }
    }
}
