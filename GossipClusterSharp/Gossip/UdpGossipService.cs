using Dignus.Collections;
using GossipClusterSharp.Cluster;
using GossipClusterSharp.Gossip.Interfaces;
using GossipClusterSharp.Gossip.Transport;
using System.Net;
using System.Text;
using System.Text.Json;

namespace GossipClusterSharp.Gossip
{
    public delegate Task GossipMessageHandler(byte[] message, IPEndPoint senderEndPoint);

    public class UdpGossipService : IGossipService
    {
        private readonly IGossipUdpTransport _gossipTransport;
        private readonly INodeRegistry _nodeRegistry;

        private readonly ArrayQueue<byte> _receiveBuffer = [];
        private readonly GossipNode _localNode;

        private bool _isRunning;

        private readonly Dictionary<string, Func<GossipMessage, IPEndPoint, Task>> _customHandlers = [];
        public UdpGossipService(
            int port,
            INodeRegistry nodeRegistry)
        {
            _nodeRegistry = nodeRegistry;
            _gossipTransport = new UdpGossipTransport(port);
            _gossipTransport.MessageReceived += OnMessageReceivedAsync;

            var ipEndPoint = _gossipTransport.GetIPEndPoint();
            _localNode = new GossipNode(ipEndPoint.Address.ToString(), ipEndPoint.Port);

            RegisterMessageHandler("Ping", HandlePingAsync);
            RegisterMessageHandler("Pong", HandlePongAsync);
        }
        public void RegisterMessageHandler(string messageType, Func<GossipMessage, IPEndPoint, Task> handler)
        {
            _customHandlers.Add(messageType, handler);
        }

        public async Task StopAsync()
        {
            await _gossipTransport.StopAsync();
        }
        private async Task HandlePingAsync(GossipMessage message, IPEndPoint senderEndPoint)
        {
            var pingMessage = message.GetPayload<PingMessage>();
            if (pingMessage == null)
            {
                return;
            }

            var pongMessage = GossipMessage.FromPayload(GossipType.Pong, new PongMessage()
            {
                RespondingNodeId = _localNode.NodeId,
            });

            await _gossipTransport.SendMessageAsync(pongMessage.ToPacket(), senderEndPoint);
        }
        private Task HandlePongAsync(GossipMessage message, IPEndPoint _)
        {
            var pongMessage = message.GetPayload<PongMessage>();
            if (pongMessage == null)
            {
                return Task.CompletedTask;
            }

            var node = _nodeRegistry.GetNode(pongMessage.RespondingNodeId);
            if (node != null)
            {
                node.UpdateHeartbeat();
                node.IsAlive = true;
            }

            return Task.CompletedTask;
        }

        private async Task OnMessageReceivedAsync(byte[] bytes, IPEndPoint senderEndPoint)
        {
            _receiveBuffer.AddRange(bytes);
            var headerSize = sizeof(int);

            if (_receiveBuffer.Count < headerSize)
            {
                return;
            }

            var packetSizeBytes = _receiveBuffer.Peek(headerSize);

            var payloadSize = BitConverter.ToInt32(packetSizeBytes);

            if (payloadSize > _receiveBuffer.Count)
            {
                return;
            }

            _receiveBuffer.Read(headerSize);
            var payloadBytes = _receiveBuffer.Read(payloadSize);
            var jsonString = Encoding.UTF8.GetString(payloadBytes);
            GossipMessage gossipMessage;
            try
            {
                gossipMessage = JsonSerializer.Deserialize<GossipMessage>(jsonString);
            }
            catch (Exception)
            {
                return;
            }

            foreach (var node in gossipMessage.GossipNodes)
            {
                var findNode = _nodeRegistry.GetNode(node.NodeId);
                if (findNode == null)
                {
                    _nodeRegistry.RegisterNode(new GossipNode(node.Ip, node.Port));
                }
            }

            if (_customHandlers.TryGetValue(gossipMessage.MessageType, out Func<GossipMessage, IPEndPoint, Task> handler) == true)
            {
                await handler(gossipMessage, senderEndPoint);
            }
        }
        private async Task StartPingingAsync()
        {
            while (true)
            {
                var targetNodes = _nodeRegistry.GetRandomNode(2);
                if (targetNodes != null)
                {
                    var knownNodes = _nodeRegistry.GetRandomNode(3);

                    foreach (var node in targetNodes)
                    {
                        var pingMessage = GossipMessage.FromPayload(GossipType.Ping, new PingMessage());

                        pingMessage.GossipNodes.AddRange(knownNodes);

                        await _gossipTransport.SendMessageAsync(pingMessage.ToPacket(),
                            node.EndPoint);
                    }
                }
                await Task.Delay(5000);
            }

        }
        private Task StartListeningAsync()
        {
            var startListeningTask = _gossipTransport.StartListeningAsync();
            _ = StartPingingAsync();
            return startListeningTask;
        }

        public Task StartAsync()
        {
            if (_isRunning == true)
            {
                throw new InvalidOperationException("gossip service is already started.");
            }
            _isRunning = true;
            return StartListeningAsync();
        }

        public GossipNode GetLocalNode()
        {
            return _localNode;
        }

        public Task SendAsync(GossipMessage message, GossipNode targetNode)
        {
            return _gossipTransport.SendMessageAsync(message.ToPacket(), targetNode.EndPoint);
        }
    }
}
