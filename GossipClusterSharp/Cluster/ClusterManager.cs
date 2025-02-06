using GossipClusterSharp.Gossip;

namespace GossipClusterSharp.Cluster
{
    public class ClusterManager
    {
        private readonly INodeRegistry _nodeRegistry;
        private NodeState _currentMasterNode;
        private List<GossipService> _gossipServices;
        private static readonly long _suspectTimeoutTicks = TimeSpan.FromSeconds(30).Ticks;
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

            await MonitorNodesAsync();
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

        private async Task MonitorNodesAsync()
        {
            while (true)
            {
                await DetectFailuresAsync();
                await Task.Delay(5000);
            }
        }

        private async Task DetectFailuresAsync()
        {
            foreach (var node in _nodeRegistry.GetAllNodeStates())
            {
                if (node.IsTimeout(_suspectTimeoutTicks))
                {
                    node.IsSuspected = true;
                }
                else if (node.IsTimeout(_suspectTimeoutTicks) && node.IsSuspected)
                {
                    node.IsAlive = false;
                }
                if (node.IsMaster == true && node.IsAlive == false)
                {
                    await ElectMasterNodeAsync();
                }
            }
        }


    }
}
