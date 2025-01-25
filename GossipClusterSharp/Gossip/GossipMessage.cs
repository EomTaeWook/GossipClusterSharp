namespace GossipClusterSharp.Gossip
{
    public class GossipMessage
    {
        public string NodeId { get; }
        public string MessageType { get; }
        public DateTime Timestamp { get; }

        public GossipMessage(string nodeId, string messageType)
        {
            NodeId = nodeId;
            MessageType = messageType;
            Timestamp = DateTime.UtcNow;
        }
    }
}
