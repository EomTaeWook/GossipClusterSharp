using GossipClusterSharp.Gossip.Interfaces;

namespace GossipClusterSharp.Cluster
{
    public class ClusterManager
    {
        private const int FailureDetectionTimeout = 30;

        private readonly INodeRegistry _nodeRegistry;
        private readonly List<IGossipService> _gossipServices;

        public ClusterManager(INodeRegistry nodeRegistry, List<IGossipService> gossipServices)
        {
            _nodeRegistry = nodeRegistry;
            _gossipServices = gossipServices;
        }

        public async Task InitializeClusterAsync()
        {
            foreach (var gossipService in _gossipServices)
            {
                _ = gossipService.StartAsync();
            }

            await StartNodeMonitoringAsync();
        }
        private async Task StartNodeMonitoringAsync()
        {
            while (true)
            {
                CheckNodeHealthAsync();
                await Task.Delay(5000);
            }
        }
        private void CheckNodeHealthAsync()
        {
            var now = DateTime.UtcNow;
            foreach (var node in _nodeRegistry.GetAllNodes())
            {
                if ((now - node.LastHeartbeat).TotalSeconds > FailureDetectionTimeout)
                {
                    node.IsAlive = false;
                }
            }
        }
    }
}
