using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace GossipClusterSharp.Gossip
{
    public class GossipNode
    {
        public string NodeId { get; private set; }
        public string Ip { get; }
        public int Port { get; }
        public bool IsAlive { get; set; } = true;
        public DateTime LastHeartbeat { get; private set; } = DateTime.UtcNow;

        public IPEndPoint EndPoint { get; private set; }
        public GossipNode(string ipString, int port)
        {
            NodeId = GenerateNodeId(ipString, port);
            Ip = ipString;
            Port = port;
            EndPoint = new IPEndPoint(IPAddress.Parse(ipString), port);
        }
        public void UpdateHeartbeat()
        {
            LastHeartbeat = DateTime.UtcNow;
        }

        private string GenerateNodeId(string ip, int port)
        {
            string input = $"{ip}:{port}";
            byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hash).Replace("-", "")[..16];
        }
    }
}
