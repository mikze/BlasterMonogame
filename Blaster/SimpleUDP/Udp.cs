using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SimpleUDP
{
    public abstract class Udp
    {
        public UdpClient listener { get; set; }

        public Udp(int port)
        {
            listener = new UdpClient(port);
        }

        public virtual byte[] Listen(ref IPEndPoint endPoint)
        {
            while (true)
            {
                var rec = listener.ReceiveAsync();
                rec.Wait();
                var result = rec.Result;
                endPoint = result.RemoteEndPoint;
                return result.Buffer;
            }
        }

        public virtual void Send(byte [] msg, IPEndPoint host) => listener.Send(msg, msg.Length, host);
    }
}
