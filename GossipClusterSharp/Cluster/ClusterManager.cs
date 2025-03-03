using Dignus.Collections;
using GossipClusterSharp.Gossip;
using GossipClusterSharp.Gossip.Interfaces;

namespace GossipClusterSharp.Cluster
{
    public class ClusterManager
    {
        public INodeRegistry NodeRegistry => _nodeRegistry;
        private const int FailureDetectionTimeout = 30;

        private readonly INodeRegistry _nodeRegistry;
        private readonly ArrayQueue<IGossipService> _gossipServices = [];

        public ClusterManager(INodeRegistry nodeRegistry)
        {
            _nodeRegistry = nodeRegistry;
        }
        public void AddGossipService(IGossipService gossipService)
        {
            _gossipServices.Add(gossipService);
            AddGossipNode(gossipService.GetLocalNode());
        }
        public void AddGossipNode(GossipNode gossipNode)
        {
            _nodeRegistry.RegisterNode(gossipNode);
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
