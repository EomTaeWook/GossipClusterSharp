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
        public GossipMessage(string messageType, string payloadJson)
        {
            MessageType = messageType;
            Timestamp = DateTime.UtcNow;

            PayloadJson = payloadJson;
        }
        public T GetPayload<T>() where T : IGossipPayload
        {
            return JsonSerializer.Deserialize<T>(PayloadJson);
        }
        public static GossipMessage FromPayload<T>(string messageType, T payload) where T : IGossipPayload
        {
            return new GossipMessage(messageType, JsonSerializer.Serialize(payload));
        }
        public Packet ToPacket()
        {
            var json = JsonSerializer.Serialize(this);
            return new Packet(Encoding.UTF8.GetBytes(json));
        }
    }

    public class PingMessage : IGossipPayload
    {
        public string TargetNodeId { get; set; }
    }
    public class PongMessage : IGossipPayload
    {
        public string TargetNodeId { get; set; }
    }
    public class MasterElectionMessage : IGossipPayload
    {
        public string MasterNodeId { get; set; }
    }
}
