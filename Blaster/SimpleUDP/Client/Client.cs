using Newtonsoft.Json;
using SimpleConnection.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SimpleConnection.Client
{
    public class Client<t> : ConnectionHandler
    {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
        public Client(IConnectionHandler connHandler) : base(connHandler) {}

        public t Listen() =>
            JsonConvert.DeserializeObject<t>(
                Encoding.ASCII.GetString(
                    Convert.FromBase64String(
                        Encoding.ASCII.GetString(
                            base.Listen(ref endPoint
                                )))));

        public void Send(t msg, IPEndPoint server)
        {
            var toSend = Helper.ParseObjToSend(msg);
            base.Send(toSend, server);
        }
        public void Connect(IPEndPoint server) => Send(Helper.ParseObjToSend(new Ramka() { msg = "Welcome" }), server);
        public void Disconnect(IPEndPoint server) => Send(Helper.ParseObjToSend(new Ramka() { msg = "Disconnect" }), server);
    }
}
