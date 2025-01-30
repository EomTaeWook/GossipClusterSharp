using Dignus.Sockets;
using Dignus.Sockets.Interfaces;
using GossipClusterSharp.Gossip;

namespace App.Node
{
    internal class TestServer : ServerBase
    {
        private GossipService _gossipService;
        public TestServer(SessionConfiguration SessionConfiguration,
            GossipService gossipService) : base(SessionConfiguration)
        {
            _gossipService = gossipService;


        }
        public Task StartAsync(int serverPort)
        {
            Start(serverPort);

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
