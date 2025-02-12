using GossipClusterSharp.Gossip.Interfaces;
using GossipClusterSharp.Networks;
using System.Net.Sockets;

namespace GossipClusterSharp.Gossip
{
    public class UdpGossipTransport : IGossipTransport
    {
        private readonly UdpClient _udpClient;

        public event GossipMessageHandler MessageReceived;

        public UdpGossipTransport(int port)
        {
            _udpClient = new UdpClient(port);
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
        public async Task SendMessageAsync(Packet packet, string endPoint)
        {
            var parts = endPoint.Split(':');
            if (parts.Length != 2 || !int.TryParse(parts[1], out var port))
            {
                throw new ArgumentException("invalid endpoint format. expected format: IP:Port");
            }
            await _udpClient.SendAsync(packet.ToByteArray(), parts[0], port);
        }
    }
}
