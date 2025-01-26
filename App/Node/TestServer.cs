using Dignus.Sockets;
using Dignus.Sockets.Interfaces;
using GossipClusterSharp.Gossip;
using GossipClusterSharp.Gossip.Interfaces;

namespace App.Node
{
    internal class TestServer : ServerBase, INodeState
    {
        public string NodeId { get; private set; }
        public bool IsAlive { get; set; }
        public bool IsMaster { get; set; }
        public int Priority { get; private set; }
        public string Endpoint { get; private set; }

        private GossipService _gossipService;
        public TestServer(SessionConfiguration SessionConfiguration,
            string nodeId,
            string endpoint) : base(SessionConfiguration)
        {
            NodeId = nodeId;
            Endpoint = endpoint;
        }
        public Task StartAsync(int serverPort)
        {
            Start(serverPort, ProtocolType.Udp);
            _ = _gossipService.StartListeningAsync();

            return Task.CompletedTask;
        }

        protected override void OnAccepted(ISession session)
        {

        }

        protected override void OnDisconnected(ISession session)
        {

        }
    }
}
