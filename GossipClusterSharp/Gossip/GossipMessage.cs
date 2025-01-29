namespace GossipClusterSharp.Gossip
{
    public class GossipMessage
    {
        public string MessageType { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string Payload { get; private set; }

        public GossipMessage(string messageType, string payload = "")
        {
            MessageType = messageType;
            Timestamp = DateTime.UtcNow;
            Payload = payload;
        }
    }
}
