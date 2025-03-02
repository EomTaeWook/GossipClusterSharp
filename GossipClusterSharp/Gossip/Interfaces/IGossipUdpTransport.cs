using GossipClusterSharp.Gossip.Transport;
using GossipClusterSharp.Networks;
using System.Net;

namespace GossipClusterSharp.Gossip.Interfaces
{
    public interface IGossipUdpTransport : IDisposable
    {
        public event GossipMessageHandler MessageReceived;
        Task SendMessageAsync(Packet packet, IPEndPoint targetEndPoint);
        Task StartListeningAsync();
        Task StopAsync();
        public IPEndPoint GetIPEndPoint();
    }
}
