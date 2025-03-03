// See https://aka.ms/new-console-template for more information
using App.Node;
using App.Serializer;
using Dignus.Sockets;
using Dignus.Sockets.Interfaces;
using GossipClusterSharp.Cluster;
using GossipClusterSharp.Gossip;



var configuration = new SessionConfiguration(() =>
{
    return new Tuple<IPacketSerializer, IPacketDeserializer, ICollection<ISessionComponent>>(new PacketSerializer(), new PacketDeserializer(), new List<ISessionComponent>());
});


var nodeRegistry = new NodeRegistry();
nodeRegistry.RegisterNode(new GossipNode("127.0.0.1", 10081));


var gossipService1 = new UdpGossipService(10081, nodeRegistry);
var gossipService2 = new UdpGossipService(10082, nodeRegistry);


// ClusterManager 생성 및 초기화
var clusterManager = new ClusterManager(nodeRegistry);
_ = clusterManager.InitializeClusterAsync();

var node1 = new TestServer(configuration, gossipService1);
var node2 = new TestServer(configuration, gossipService2);

_ = node1.StartAsync(9081);
_ = node2.StartAsync(9082);
Console.WriteLine("Cluster is running. Press Enter to exit...");
Console.ReadLine();
