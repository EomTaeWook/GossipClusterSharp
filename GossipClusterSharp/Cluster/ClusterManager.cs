using GossipClusterSharp.Gossip;

namespace GossipClusterSharp.Cluster
{
    internal class ClusterManager
    {
        private readonly GossipService _gossipService;
        private INodeRegistry _nodeRegistry;
        public ClusterManager(GossipService gossipService)
        {
            _gossipService = gossipService;
        }

        public void InitializeClusterAsync()
        {
            _nodeRegistry.GetAllNodeStates();

        }
        private async Task StartListeningAsync()
        {




        }

    }
}
