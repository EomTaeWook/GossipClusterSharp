using GossipClusterSharp.Cluster;

namespace GossipClusterSharp.Communication
{
    internal class StateSyncService
    {
        private readonly Dictionary<string, ClusterState> _clusterStates = new();

    }
}
