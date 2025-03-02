using GossipClusterSharp.Networks;
using System.Text;
using System.Text.Json;

namespace GossipClusterSharp.Gossip
{
    public interface IGossipPayload
    {
    }

    public class GossipMessage
    {
        public string MessageType { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string PayloadJson { get; private set; }

        public List<GossipNode> GossipNodes { get; set; }
        public GossipMessage(string messageType, string payloadJson, List<GossipNode> gossipNodes)
        {
            MessageType = messageType;
            Timestamp = DateTime.UtcNow;
            PayloadJson = payloadJson;
            GossipNodes = gossipNodes;
        }
        public T GetPayload<T>() where T : IGossipPayload
        {
            return JsonSerializer.Deserialize<T>(PayloadJson);
        }
        public static GossipMessage FromPayload<T>(GossipType messageType, T payload, List<GossipNode> gossipNodes = null) where T : IGossipPayload
        {
            return FromPayload(messageType.ToString(), payload, gossipNodes);
        }
        public static GossipMessage FromPayload<T>(string messageType, T payload, List<GossipNode> gossipNodes = null) where T : IGossipPayload
        {
            return new GossipMessage(messageType, JsonSerializer.Serialize(payload), gossipNodes);
        }
        public Packet ToPacket()
        {
            var json = JsonSerializer.Serialize(this);
            return new Packet(Encoding.UTF8.GetBytes(json));
        }
    }

    public class PingMessage : IGossipPayload
    {
        public string RespondingNodeId { get; set; }
    }
    public class PongMessage : IGossipPayload
    {
        public string RespondingNodeId { get; set; }
    }
}
