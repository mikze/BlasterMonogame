using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SimpleUDP.Server
{
    public class Client : IClient
    {
        static int _id = 0;

        public Client(IPEndPoint EndPoint)
        {
            this.EndPoint = EndPoint;
            Id = ++_id;
        }
        public IPEndPoint EndPoint { get; set; }
        public int Id { get; }

        public void ClientRemove()
        {      
            _id--;
        }
    }
}
