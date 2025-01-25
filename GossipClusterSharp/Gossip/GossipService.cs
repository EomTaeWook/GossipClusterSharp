using GossipClusterSharp.Cluster;
using GossipClusterSharp.Gossip.Interfaces;

namespace GossipClusterSharp.Gossip
{
    internal class GossipService
    {
        private string _representativeNode;
        private IGossipTransport _gossipTransport;
        private readonly INodeRegistry _nodeRegistry;
        private Action<GossipMessage> _receiveMessageCallback;
        public GossipService(IGossipTransport gossipTransports, INodeRegistry nodeRegistry)
        {
            _nodeRegistry = nodeRegistry;
            _gossipTransport = gossipTransports;
        }

        public Task StartListeningAsync(Action<GossipMessage> receiveMessageCallback)
        {
            _receiveMessageCallback = receiveMessageCallback;
            _gossipTransport.MessageReceived += OnMessageReceived;

            return _gossipTransport.StartListeningAsync();
        }

        private void OnMessageReceived(GossipMessage message)
        {
            _receiveMessageCallback?.Invoke(message);
        }

        public async Task SendMessageToRandomNodeAsync(GossipMessage message)
        {
            var targetNode = _nodeRegistry.GetRandomNode();
            if (targetNode == null)
            {
                return;
            }
            await _gossipTransport.SendMessageAsync(message, targetNode.Endpoint);
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
