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

        public t Listen()
        {
            var frame = Helper.EncapsulateFromFrame(Encoding.ASCII.GetString(
                    Convert.FromBase64String(
                        Encoding.ASCII.GetString(
                            base.Listen(ref endPoint
                                )))));
            if(!string.IsNullOrEmpty(frame.ackHash))
            {
                var Frame = JsonConvert.SerializeObject(new Ramka() { msg = "ack", ackHash = frame.ackHash });
                var base64 = Convert.ToBase64String(Encoding.ASCII.GetBytes(Frame));
                var toSend = Encoding.ASCII.GetBytes(base64);
                base.Send(toSend, endPoint);
            }
            var encapsulatedFrame = frame.msg;

            return JsonConvert.DeserializeObject<t>(encapsulatedFrame);
        }

        public void Send(t msg, IPEndPoint server)
        {
            var toSend = Helper.ParseObjToSend(msg, string.Empty);
            base.Send(toSend, server);
        }
        public void Connect(IPEndPoint server) => Send(Helper.ParseObjToSend(new Ramka() { msg = "Welcome", ackHash = string.Empty }), server);
        public void Disconnect(IPEndPoint server) => Send(Helper.ParseObjToSend(new Ramka() { msg = "Disconnect", ackHash = string.Empty }), server);
    }
}
