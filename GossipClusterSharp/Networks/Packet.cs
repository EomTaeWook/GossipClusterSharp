using Dignus.Collections;

namespace GossipClusterSharp.Networks
{
    public class Packet
    {
        public int PayloadSize { get; private set; }
        public byte[] Data { get; private set; }

        public Packet(byte[] data)
        {
            PayloadSize = data.Length;
            Data = data;
        }

        public byte[] ToByteArray()
        {
            var packetBuffer = new ArrayQueue<byte>(sizeof(int) + PayloadSize);
            packetBuffer.AddRange(BitConverter.GetBytes(PayloadSize));
            packetBuffer.AddRange(Data);
            return packetBuffer.ToArray();
        }
    }
}
