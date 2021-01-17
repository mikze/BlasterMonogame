using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SimpleConnection.Interfaces
{
    public interface IConnectionHandler : IDisposable
    {
        public void Send(byte[] msg, IPEndPoint host);
        public byte[] Listen(ref IPEndPoint endPoint);
    }
}
