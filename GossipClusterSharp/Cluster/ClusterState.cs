namespace GossipClusterSharp.Cluster
{
    internal class ClusterState
    {
        public string ClusterId { get; }
        public int AliveNodes { get; private set; }
        public DateTime LastUpdated { get; private set; }

        public ClusterState(string clusterId, int aliveNodes)
        {
            ClusterId = clusterId;
            AliveNodes = aliveNodes;
            LastUpdated = DateTime.UtcNow;
        }
        public void UpdateAliveNodes(int aliveNodes)
        {
            AliveNodes = aliveNodes;
            UpdateTime(); // 노드 수가 변경되면 자동으로 시간 업데이트
        }

        public void UpdateTime()
        {
            LastUpdated = DateTime.UtcNow;
        }
        public int CompareTo(ClusterState other)
        {
            if (other == null)
            {
                return 1;
            }
            return AliveNodes.CompareTo(other.AliveNodes);
        }
    }
}
