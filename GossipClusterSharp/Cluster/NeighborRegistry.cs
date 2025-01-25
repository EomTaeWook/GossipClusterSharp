using GossipClusterSharp.Gossip.Interfaces;

namespace GossipClusterSharp.Cluster
{
    public class NeighborRegistry
    {
        public string GroupId { get; }
        private readonly NodeRegistry _nodeRegistry;

        public NeighborRegistry(string groupId)
        {
            GroupId = groupId;
            _nodeRegistry = new NodeRegistry();
        }

        public void AddNode(INodeState node)
        {
            _nodeRegistry.RegisterNode(node);
        }
        public void RemoveNode(string nodeId)
        {
            _nodeRegistry.RemoveNode(nodeId);
        }
        public int GetAliveNodeCount()
        {
            return _nodeRegistry.GetAliveNodeCount();
        }

        public INodeState ElectMasterNode()
        {
            var aliveNodes = _nodeRegistry.GetAllNodeStates().Where(n => n.IsAlive).ToList();
            if (aliveNodes.Count == 0)
            {
                return null;
            }

            foreach (var node in _nodeRegistry.GetAllNodeStates())
            {
                node.IsMaster = false;
            }

            var newMaster = aliveNodes.First();
            newMaster.IsMaster = true;

            return newMaster;
        }
    }
}
