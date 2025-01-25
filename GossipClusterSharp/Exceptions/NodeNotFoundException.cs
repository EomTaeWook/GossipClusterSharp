namespace GossipClusterSharp.Exceptions
{
    public class NodeNotFoundException : Exception
    {
        public NodeNotFoundException(string nodeId)
            : base($"node {nodeId} is not found in the cluster.")
        {
        }
    }
}
