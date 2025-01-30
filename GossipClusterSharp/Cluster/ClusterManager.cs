using GossipClusterSharp.Gossip;

namespace GossipClusterSharp.Cluster
{
    public class ClusterManager
    {
        private readonly INodeRegistry _nodeRegistry;
        private NodeState _currentMasterNode;
        private List<GossipService> _gossipServices;
        public ClusterManager(INodeRegistry nodeRegistry, List<GossipService> gossipServices)
        {
            _nodeRegistry = nodeRegistry;
            _gossipServices = gossipServices;
        }

        public async Task InitializeClusterAsync()
        {
            foreach (var gossipService in _gossipServices)
            {
                _ = gossipService.StartListeningAsync();
            }

            await ElectMasterNodeAsync();
        }
        public void DetectFailures()
        {
            foreach (var node in _nodeRegistry.GetAllNodeStates())
            {
                double elapsedSeconds = new TimeSpan(DateTime.UtcNow.Ticks - node.LastUpdatedTicks).TotalSeconds;
                if (node.IsSuspected == false && elapsedSeconds > 10)
                {
                    node.IsSuspected = true;
                }

                if (node.IsSuspected && elapsedSeconds > 20)
                {
                    node.IsAlive = false;

                    if (_currentMasterNode != null && _currentMasterNode.NodeId == node.NodeId)
                    {
                        _ = ElectMasterNodeAsync();
                    }
                }
            }
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
                throw new InvalidOperationException("no alive nodes available for master election. cluster in DEGRADED state.");
            }

            var newMaster = aliveNodes.First();
            _currentMasterNode = newMaster;

            var message = new GossipMessage(GossipType.MasterElection.ToString(), newMaster.NodeId);
            foreach (var gossipService in _gossipServices)
            {
                await gossipService.BroadcastToAllNodesAsync(message);
            }
        }
    }
}
