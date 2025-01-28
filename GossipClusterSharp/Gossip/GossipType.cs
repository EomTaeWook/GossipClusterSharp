namespace GossipClusterSharp.Gossip
{
    internal enum GossipType
    {
        None,

        Heartbeat,
        MasterFailure,
        MasterElection,
        StateUpdate,
        ClusterElection,
    }
}
