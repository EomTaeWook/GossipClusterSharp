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
        public string PayloadJson { get; set; }
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
