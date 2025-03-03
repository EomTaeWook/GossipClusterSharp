using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace GossipClusterSharp.Gossip
{
    public class GossipNode
    {
        public string NodeId { get; init; }
        public string Ip { get; init; }
        public int Port { get; init; }
        public bool IsAlive { get; set; } = true;
        [JsonIgnore]
        public DateTime LastHeartbeat { get; private set; } = DateTime.UtcNow;

        private readonly IPEndPoint _ipEndPoint;
        public GossipNode()
        {
        }
        public GossipNode(string ipString, int port)
        {
            NodeId = GenerateNodeId(ipString, port);
            Ip = ipString;
            Port = port;
            _ipEndPoint = new IPEndPoint(IPAddress.Parse(ipString), port);
        }

        public IPEndPoint GetEndPoint()
        {
            return _ipEndPoint;
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
