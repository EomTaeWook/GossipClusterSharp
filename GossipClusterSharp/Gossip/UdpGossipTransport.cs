using GossipClusterSharp.Gossip.Interfaces;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

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
        public async Task SendMessageAsync(GossipMessage message, string endPoint)
        {
            var parts = endPoint.Split(':');
            if (parts.Length != 2 || !int.TryParse(parts[1], out var port))
            {
                throw new ArgumentException("invalid endpoint format. Expected format: IP:Port");
            }
            var payload = JsonSerializer.Serialize(message);
            var payloadBytes = Encoding.UTF8.GetBytes(payload);

            await _udpClient.SendAsync(payloadBytes, payloadBytes.Length, parts[0], port);
        }
    }
}
