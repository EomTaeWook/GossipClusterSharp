using GossipClusterSharp.Gossip.Interfaces;

namespace GossipClusterSharp.Gossip
{
    public class NodeState : INodeState
    {
        public string NodeId { get; set; }
        public bool IsAlive { get; set; } = true;
        public bool IsMaster { get; set; }
        public int Priority { get; set; }
        public string Endpoint { get; set; }
        public long LastUpdatedTicks { get; private set; }
        public NodeState(string nodeId, int priority, string endPoint)
        {
            NodeId = nodeId;
            Priority = priority;
            Endpoint = endPoint;
        }

        public void UpdateState(bool isAlive, bool isMaster)
        {
            IsAlive = isAlive;
            IsMaster = isMaster;
            LastUpdatedTicks = DateTime.UtcNow.Ticks;
        }
    }
}
