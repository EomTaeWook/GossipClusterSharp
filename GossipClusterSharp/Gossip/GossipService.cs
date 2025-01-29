using GossipClusterSharp.Cluster;
using GossipClusterSharp.Gossip.Interfaces;
using System.Net;
using System.Text;
using System.Text.Json;

namespace GossipClusterSharp.Gossip
{
    public delegate Task GossipMessageHandler(byte[] message, IPEndPoint senderEndPoint);

    public class GossipService
    {
        private readonly IGossipTransport _gossipTransport;
        private readonly INodeRegistry _nodeRegistry;

        public GossipService(
            IGossipTransport gossipTransports,
            INodeRegistry nodeRegistry)
        {
            _nodeRegistry = nodeRegistry;
            _gossipTransport = gossipTransports;

            _gossipTransport.MessageReceived += OnMessageReceivedAsync;
        }

        private async Task OnMessageReceivedAsync(byte[] message, IPEndPoint senderEndPoint)
        {
            var json = Encoding.UTF8.GetString(message);
            GossipMessage gossipMessage;
            try
            {
                gossipMessage = JsonSerializer.Deserialize<GossipMessage>(json);
            }
            catch (Exception ex)
            {
                return;
            }

            if (Enum.TryParse<GossipType>(gossipMessage.MessageType, out GossipType gossipType) == false)
            {
                //MessageReceived?.Invoke(message);
                return;
            }

            switch (gossipType)
            {
                case GossipType.Ping:

                    var pingMessage = gossipMessage.GetPayload<PingMessage>();

                    var pongMessage = GossipMessage.FromPayload(GossipType.Pong.ToString(), new PongMessage()
                    {
                        SenderNodeId = pingMessage.SenderNodeId
                    });

                    await _gossipTransport.SendMessageAsync(pongMessage, senderEndPoint.ToString());

                    break;

                case GossipType.Pong:
                    var pongPayload = gossipMessage.GetPayload<PongMessage>();
                    if (pongPayload != null)
                    {
                        UpdateHeartBeat(pongPayload.SenderNodeId);
                    }
                    break;
                default:
                    break;
            }
        }
        private void UpdateHeartBeat(string nodeId)
        {
            var node = _nodeRegistry.GetNodeState(nodeId);
            if (node != null)
            {
                node.IncrementHeartbeat();
            }
        }
        private async Task StartPingingAsync()
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

        //public async Task BroadcastMasterFailureAsync(string masterNodeId)
        //{
        //    var message = new GossipMessage(masterNodeId, GossipType.MasterFailure.ToString());
        //    await BroadcastToAllNodesAsync(message);
        //}
    }
}
