namespace GossipClusterSharp.Exceptions
{
    public class NodeAlreadyExistsException : Exception
    {
        public NodeAlreadyExistsException(string nodeId)
            : base($"node {nodeId} is already registered in the cluster.")
        {
        }

    }
}
