using GossipClusterSharp.Cluster;
using GossipClusterSharp.Gossip.Interfaces;

namespace GossipClusterSharp.Gossip
{
    public class GossipService
    {
        private readonly IGossipTransport _gossipTransport;
        private readonly INodeRegistry _nodeRegistry;

        public GossipService(IGossipTransport gossipTransports,
            INodeRegistry nodeRegistry)
        {
            _nodeRegistry = nodeRegistry;
            _gossipTransport = gossipTransports;

            _gossipTransport.MessageReceived += OnMessageReceived;
        }

        private void OnMessageReceived(GossipMessage message)
        {

        }

        public Task StartListeningAsync()
        {
            return _gossipTransport.StartListeningAsync();
        }

        public async Task SendMessageToRandomNodeAsync(GossipMessage message)
        {
            var targetNodes = _nodeRegistry.GetRandomNode();
            if (targetNodes == null)
            {
                return;
            }
            foreach (var targetNode in targetNodes)
            {
                await _gossipTransport.SendMessageAsync(message, targetNode.Endpoint);
            }
        }

        public async Task BroadcastToAllNodesAsync(GossipMessage message)
        {
            foreach (var node in _nodeRegistry.GetAllNodeStates())
            {
                if (node.IsAlive == false)
                {
                    continue;
                }
                await _gossipTransport.SendMessageAsync(message, node.Endpoint);
            }
        }

        public async Task BroadcastMasterFailureAsync(string masterNodeId)
        {
            var message = new GossipMessage(masterNodeId, GossipType.MasterFailure.ToString());
            await BroadcastToAllNodesAsync(message);
        }
    }
}
