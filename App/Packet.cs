using Dignus.Sockets.Interfaces;

namespace App
{
    internal class Packet : IPacket
    {
        private readonly byte[] _body;
        public Packet(byte[] body)
        {
            _body = body;
        }
        public int GetLength()
        {
            return _body.Length;
        }
        public byte[] GetBody()
        {
            return _body;
        }
    }
}
