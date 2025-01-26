using Dignus.Collections;
using Dignus.Sockets.Interfaces;

namespace App.Serializer
{
    internal class PacketDeserializer : IPacketDeserializer
    {
        public void Deserialize(ArrayQueue<byte> buffer)
        {
            var packetSizeBytes = buffer.Read(sizeof(int));
            var packetSize = BitConverter.ToInt32(packetSizeBytes);

            var bodyBytes = buffer.Read(packetSize);




        }

        public bool IsCompletePacketInBuffer(ArrayQueue<byte> buffer)
        {
            var packetSizeBytes = buffer.Peek(sizeof(int));
            var packetSize = BitConverter.ToInt32(packetSizeBytes);

            return packetSize <= buffer.Count;
        }
    }
}
