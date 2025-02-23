using GossipClusterSharp.Gossip.Interfaces;
using GossipClusterSharp.Networks;
using System.Net;
using System.Net.Sockets;

namespace GossipClusterSharp.Gossip
{
    public class UdpGossipTransport : IGossipUdpTransport
    {
        private readonly UdpClient _udpClient;

        public event GossipMessageHandler MessageReceived;

        private readonly int _port;
        public UdpGossipTransport(int port)
        {
            _udpClient = new UdpClient(port);
            _port = port;
        }

        public IPEndPoint GetIPEndPoint()
        {
            return new IPEndPoint(IPAddress.Any, _port);
        }

        public async Task StartListeningAsync()
        {
            while (true)
            {
                try
                {
                    var result = await _udpClient.ReceiveAsync();
                    MessageReceived?.Invoke(result.Buffer, result.RemoteEndPoint);
                }
                catch (SocketException socketException)
                {
                    if (socketException.SocketErrorCode == SocketError.ConnectionAborted)
                    {
                        break;
                    }
                }
            }
        }

        public async Task SendMessageAsync(Packet packet, IPEndPoint targetEndPoint)
        {
            await _udpClient.SendAsync(packet.ToByteArray(), targetEndPoint);
        }
    }
}
