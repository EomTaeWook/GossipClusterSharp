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

        private readonly Dictionary<GossipType, Func<GossipMessage, IPEndPoint, Task>> _handlers;

        public GossipService(
            IGossipTransport gossipTransports,
            INodeRegistry nodeRegistry)
        {
            _nodeRegistry = nodeRegistry;
            _gossipTransport = gossipTransports;

            _gossipTransport.MessageReceived += OnMessageReceivedAsync;

            _handlers = new Dictionary<GossipType, Func<GossipMessage, IPEndPoint, Task>>
            {
                { GossipType.Ping, HandlePingAsync },
                { GossipType.Pong, HandlePongAsync },
                { GossipType.MasterElection, HandleMasterElectionAsync }
            };
        }
        private Task HandleMasterElectionAsync(GossipMessage message, IPEndPoint senderEndPoint)
        {
            var masterElectionMessage = message.GetPayload<MasterElectionMessage>();

            if (masterElectionMessage == null)
            {
                return Task.CompletedTask;
            }

            var masterNode = _nodeRegistry.GetNodeState(masterElectionMessage.MasterNodeId);
            if (masterNode == null)
            {
                return Task.CompletedTask;
            }

            foreach (var node in _nodeRegistry.GetAllNodeStates())
            {
                node.IsMaster = false;
            }
            masterNode.IsMaster = true;

            return Task.CompletedTask;
        }

        private Task HandlePongAsync(GossipMessage message, IPEndPoint senderEndPoint)
        {
            var pongPayload = message.GetPayload<PongMessage>();
            if (pongPayload == null)
            {
                return Task.CompletedTask;
            }

            var node = _nodeRegistry.GetNodeState(pongPayload.TargetNodeId);
            if (node != null)
            {
                node.IncrementHeartbeat();
            }
            return Task.CompletedTask;
        }
        private async Task HandlePingAsync(GossipMessage message, IPEndPoint senderEndPoint)
        {
            var pingMessage = message.GetPayload<PingMessage>();
            if (pingMessage == null)
            {
                return;
            }

            var pongMessage = GossipMessage.FromPayload(GossipType.Pong.ToString(), new PongMessage()
            {
                TargetNodeId = pingMessage.TargetNodeId
            });

            await _gossipTransport.SendMessageAsync(pongMessage, senderEndPoint.ToString());
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
                return;
            }

            if (_handlers.TryGetValue(gossipType, out var handler))
            {
                await handler(gossipMessage, senderEndPoint);
            }
        }
        private async Task StartPingingAsync()
        {
            while (true)
            {
                var targetNodes = _nodeRegistry.GetRandomNode();
                if (targetNodes != null)
                {
                    foreach (var node in targetNodes)
                    {
                        var pingMessage = GossipMessage.FromPayload(GossipType.Ping.ToString(), new PingMessage()
                        {
                            TargetNodeId = node.NodeId
                        });

                        await _gossipTransport.SendMessageAsync(pingMessage, node.Endpoint);
                    }
                }
                await Task.Delay(1000);
            }

        }
        public Task StartListeningAsync()
        {
            var startListeningTask = _gossipTransport.StartListeningAsync();
            _ = StartPingingAsync();
            return startListeningTask;
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
    }
}
