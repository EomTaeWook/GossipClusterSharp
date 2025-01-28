using GossipClusterSharp.Cluster;
using GossipClusterSharp.Gossip.Interfaces;

namespace GossipClusterSharp.Gossip
{
    public delegate void GossipMessageHandler(GossipMessage message);

    public class GossipService
    {
        private readonly IGossipTransport _gossipTransport;
        private readonly INodeRegistry _nodeRegistry;

        public event GossipMessageHandler MessageReceived;
        public GossipService(IGossipTransport gossipTransports,
            INodeRegistry nodeRegistry)
        {
            _nodeRegistry = nodeRegistry;
            _gossipTransport = gossipTransports;

            _gossipTransport.MessageReceived += OnMessageReceived;
        }

        private void OnMessageReceived(GossipMessage message)
        {
            if (Enum.TryParse<GossipType>(message.MessageType, out GossipType gossipType) == false)
            {
                MessageReceived?.Invoke(message);
                return;
            }

            switch (gossipType)
            {
                case GossipType.StateUpdate:
                    UpdateNodeState(message.NodeId, true);
                    break;
                case GossipType.MasterFailure:
                    break;
                default:
                    break;
            }
        }
        private void UpdateNodeState(string nodeId, bool isAlive)
        {
            _nodeRegistry.UpdateNodeState(nodeId, isAlive);
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
