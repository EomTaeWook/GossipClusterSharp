namespace GossipClusterSharp.Gossip
{
    public class NodeState
    {
        public bool IsAlive { get; set; } = true;
        public bool IsMaster { get; set; }
        public int Priority { get; set; }
        public string Endpoint { get; set; }
        public long LastUpdatedTicks { get; set; }
        public string NodeId { get; private set; }
        public bool IsSuspected { get; set; }
        public NodeState(string nodeId, string endPoint, int priority)
        {
            NodeId = nodeId;
            Priority = priority;
            Endpoint = endPoint;
        }
        public void IncrementHeartbeat()
        {
            LastUpdatedTicks = DateTime.UtcNow.Ticks;
            IsSuspected = false;
        }
        public bool IsTimeout(long timeoutTicks)
        {
            return (DateTime.UtcNow.Ticks - LastUpdatedTicks) > timeoutTicks;
        }
    }
}
