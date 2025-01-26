using Dignus.Collections;
using Dignus.Sockets.Interfaces;

namespace App.Serializer
{
    internal class PacketSerializer : IPacketSerializer
    {
        public ArraySegment<byte> MakeSendBuffer(IPacket packet)
        {
            var packetSize = packet.GetLength();

            var sizeBytes = BitConverter.GetBytes(packetSize);

            ArrayQueue<byte> buffer = [.. sizeBytes];

            var body = (packet as Packet).GetBody();

            buffer.AddRange(body);

            return buffer.ToArray();
        }
    }
}
