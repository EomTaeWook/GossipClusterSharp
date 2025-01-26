// See https://aka.ms/new-console-template for more information
using App.Node;
using Dignus.Sockets;
using Dignus.Sockets.Interfaces;
using GossipClusterSharp.Cluster;



var configuration = new SessionConfiguration(() =>
{
    return new Tuple<IPacketSerializer, IPacketDeserializer, ICollection<ISessionComponent>>(null, null, null);
});

var node1 = new TestServer(configuration, "Node1", "127.0.0.1:10081");
var node2 = new TestServer(configuration, "Node2", "127.0.0.1:10082");

var nodeRegistry = new NodeRegistry();
nodeRegistry.RegisterNode(node1);
nodeRegistry.RegisterNode(node2);


_ = node1.StartAsync(9081);
_ = node2.StartAsync(9082);


//// GossipService 생성
//var gossipService = new GossipService(node1, nodeRegistry);

//// ClusterManager 생성 및 초기화
//var clusterManager = new ClusterManager(gossipService, nodeRegistry);
//await clusterManager.InitializeClusterAsync();

Console.WriteLine("Cluster is running. Press Enter to exit...");
Console.ReadLine();
