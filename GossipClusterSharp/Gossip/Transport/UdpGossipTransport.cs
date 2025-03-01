using GossipClusterSharp.Gossip.Interfaces;
using GossipClusterSharp.Networks;
using System.Net;
using System.Net.Sockets;

namespace GossipClusterSharp.Gossip.Transport
{
    public class UdpGossipTransport : IGossipUdpTransport
    {
        public event GossipMessageHandler MessageReceived;

        private UdpClient _udpClient;
        private int _port;
        public UdpGossipTransport(int port)
        {
            _port = port;
            _udpClient = new UdpClient(port);
        }

        public void Dispose()
        {
            _udpClient.Dispose();
            _udpClient = null;
            GC.SuppressFinalize(this);
        }

        public Task StopAsync()
        {
            if (_udpClient != null)
            {
                _udpClient.Close();
            }

            return Task.CompletedTask;
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
