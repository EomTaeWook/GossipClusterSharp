namespace GossipClusterSharp.Gossip
{
    internal enum GossipType
    {
        None,

        MasterFailure,
        MasterElection,
        StateUpdate,
        ClusterElection,
    }
}
