using GossipClusterSharp.Gossip;

namespace GossipClusterSharp.Cluster
{
    internal class ClusterManager
    {
        private readonly List<GossipService> _gossipServices;
        private readonly INodeRegistry _nodeRegistry;
        private string _currentMasterNode;
        public ClusterManager(List<GossipService> gossipServices, INodeRegistry nodeRegistry)
        {
            _gossipServices = gossipServices;
            _nodeRegistry = nodeRegistry;
        }

        public void InitializeClusterAsync()
        {
            _ = ElectMasterNodeAsync();
        }
        private Task StartListeningAsync()
        {
            foreach (var gossipService in _gossipServices)
            {
                _ = gossipService.StartListeningAsync(async message =>
                {
                    await HandleMessageAsync(message);
                });
            }
            return Task.CompletedTask;
        }
        private async Task HandleMessageAsync(GossipMessage message)
        {
            if (Enum.TryParse<GossipType>(message.MessageType, out GossipType gossipType) == false)
            {
                return;
            }

            switch (gossipType)
            {
                case GossipType.StateUpdate:
                    //UpdateNodeState(message.NodeId, )
                    break;

                case GossipType.MasterFailure:
                    await ElectMasterNodeAsync();
                    break;

                default:
                    break;
            }

            await Task.CompletedTask;
        }
        private async Task ElectMasterNodeAsync()
        {
            var aliveNodes = _nodeRegistry.GetAllNodeStates()
                                          .Where(n => n.IsAlive)
                                          .OrderBy(n => n.Priority)
                                          .ToList();

            if (!aliveNodes.Any())
            {
                Console.WriteLine("No alive nodes available for master election.");
                return;
            }

            var newMaster = aliveNodes.First();
            _currentMasterNode = newMaster.NodeId;

            var message = new GossipMessage(newMaster.NodeId, GossipType.MasterElection.ToString());
            foreach (var gossipService in _gossipServices)
            {
                await gossipService.BroadcastToAllNodesAsync(message);
            }
        }
        private void UpdateNodeState(string nodeId, bool isAlive)
        {
            _nodeRegistry.UpdateNodeState(nodeId, isAlive);
        }
    }
}
