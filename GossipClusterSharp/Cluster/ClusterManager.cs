using GossipClusterSharp.Gossip;
using GossipClusterSharp.Gossip.Interfaces;

namespace GossipClusterSharp.Cluster
{
    public class ClusterManager
    {
        private readonly INodeRegistry _nodeRegistry;
        private INodeState _currentMasterNode;
        private List<GossipService> _gossipServices;
        public ClusterManager(INodeRegistry nodeRegistry, List<GossipService> gossipServices)
        {
            _nodeRegistry = nodeRegistry;
            _gossipServices = gossipServices;
        }

        public void InitializeClusterAsync()
        {
            _ = ElectMasterNodeAsync();
        }
        private Task StartListeningAsync()
        {
            foreach (var gossipService in _gossipServices)
            {
                _ = gossipService.StartListeningAsync();
            }
            return Task.CompletedTask;
        }
        private async Task ElectMasterNodeAsync()
        {
            var aliveNodes = _nodeRegistry.GetAllNodeStates()
                                          .Where(n => n.IsAlive)
                                          .OrderBy(n => n.Priority)
                                          .ToList();

            if (aliveNodes.Count == 0)
            {
                Console.WriteLine("no alive nodes available for master election.");
                return;
            }

            var newMaster = aliveNodes.First();
            _currentMasterNode = newMaster;

            var message = new GossipMessage(newMaster.NodeId, GossipType.MasterElection.ToString());
            foreach (var gossipService in _gossipServices)
            {
                await gossipService.BroadcastToAllNodesAsync(message);
            }
        }
        public INodeState GetCurrentMasterNode()
        {
            return _currentMasterNode;
        }

    }
}
