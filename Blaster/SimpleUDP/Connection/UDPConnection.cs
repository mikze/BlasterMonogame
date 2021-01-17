using SimpleConnection.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SimpleConnection.Connection
{
    public class UDPConnection : IConnectionHandler
    {
        public UdpClient _listener { get; set; }

        public UDPConnection(int port)
        {
            _listener = new UdpClient(port);
        }
        public byte[] Listen(ref IPEndPoint endPoint)
        {
            while (true)
            {
                var rec = _listener.ReceiveAsync();
                rec.Wait();
                var result = rec.Result;
                endPoint = result.RemoteEndPoint;
                return result.Buffer;
            }
        }

        public void Send(byte[] msg, IPEndPoint host) => _listener.Send(msg, msg.Length, host);

        public void Dispose()
        {
            _listener.Dispose();
        }
    }
}
