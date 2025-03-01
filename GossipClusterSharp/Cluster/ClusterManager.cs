using Dignus.Collections;
using GossipClusterSharp.Gossip.Interfaces;

namespace GossipClusterSharp.Cluster
{
    public class ClusterManager
    {
        private const int FailureDetectionTimeout = 30;

        private readonly INodeRegistry _nodeRegistry;
        private readonly ArrayQueue<IGossipService> _gossipServices = [];

        public ClusterManager(INodeRegistry nodeRegistry)
        {
            _nodeRegistry = nodeRegistry;
        }
        public ClusterManager(INodeRegistry nodeRegistry, List<IGossipService> gossipServices)
        {
            _nodeRegistry = nodeRegistry;
            _gossipServices.AddRange(gossipServices);
        }
        public void AddGossipService(IGossipService gossipService)
        {
            _nodeRegistry.RegisterNode(gossipService.GetLocalNode());
            _gossipServices.Add(gossipService);
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
