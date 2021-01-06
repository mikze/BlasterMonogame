using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SimpleUDP
{
    public interface IClient
    {
        public IPEndPoint EndPoint { get; set; }
        public int Id { get; }
        public void ClientRemove();
    }
}
