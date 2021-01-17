using Newtonsoft.Json;
using SimpleConnection.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SimpleConnection
{
    public abstract class ConnectionHandler
    {
        public IConnectionHandler listener { get; set; }

        public ConnectionHandler(IConnectionHandler Listener)
        {
            listener = Listener;
        }

        public virtual byte[] Listen(ref IPEndPoint endPoint) => listener.Listen(ref endPoint);

        public virtual void Send(byte [] msg, IPEndPoint host) => listener.Send(msg, host);
    }
}
