namespace GossipClusterSharp.Gossip
{
    public class NodeState
    {
        public bool IsAlive { get; set; } = true;
        public bool IsMaster { get; set; }
        public int Priority { get; set; }
        public string Endpoint { get; set; }
        public long LastUpdatedTicks { get; set; }
        public long LastHeartbeat { get; set; }
        public string NodeId { get; set; }
        public bool IsSuspected { get; set; }
        public NodeState(string endPoint, int priority)
        {
            Priority = priority;
            Endpoint = endPoint;
        }
        public void IncrementHeartbeat()
        {
            LastHeartbeat++;
            LastUpdatedTicks = DateTime.UtcNow.Ticks;
            IsSuspected = false;
        }

        public void UpdateState(bool isAlive, bool isMaster)
        {
            IsAlive = isAlive;
            IsMaster = isMaster;
            LastUpdatedTicks = DateTime.UtcNow.Ticks;
        }
    }
}
