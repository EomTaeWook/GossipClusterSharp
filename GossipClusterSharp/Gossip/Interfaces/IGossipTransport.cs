using GossipClusterSharp.Networks;
using System.Net;

namespace GossipClusterSharp.Gossip.Interfaces
{
    public interface IGossipUdpTransport
    {
        Task SendMessageAsync(Packet packet, IPEndPoint targetEndPoint);
        Task StartListeningAsync();
        public event GossipMessageHandler MessageReceived;

        public IPEndPoint GetIPEndPoint();
    }
}
