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


var gossipTransport1 = new UdpGossipTransport(10081);
var gossipTransport2 = new UdpGossipTransport(10082);

var gossipService1 = new GossipService(gossipTransport1, nodeRegistry);
var gossipService2 = new GossipService(gossipTransport2, nodeRegistry);


// ClusterManager 생성 및 초기화
var clusterManager = new ClusterManager(nodeRegistry, [gossipService1, gossipService2]);
_ = clusterManager.InitializeClusterAsync();

var node1 = new TestServer(configuration, gossipService1);
var node2 = new TestServer(configuration, gossipService2);

_ = node1.StartAsync(9081);
_ = node2.StartAsync(9082);
Console.WriteLine("Cluster is running. Press Enter to exit...");
Console.ReadLine();
