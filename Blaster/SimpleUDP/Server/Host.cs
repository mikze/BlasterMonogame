using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SimpleUDP.Server
{
    public class Host<t> : Udp
    {
        public List<IClient> clients = new List<IClient>();
        UdpClient Sender;
        public Host(int port) : base(port)
        {
            Sender = new UdpClient();
        }
        public void Listen()
        {
            while (true)
            {
                var endPoint = new IPEndPoint(IPAddress.Any, 0);
                var msg = base.Listen(ref endPoint);
                if (!IsNewClient(msg, endPoint))
                    RecieveHandler(DeParseCommand(msg), clients.FirstOrDefault(x =>x.EndPoint.Equals(endPoint)).Id);
            }
        }

        private t DeParseCommand(byte[] msg) => JsonConvert.DeserializeObject<t>(Helper.ByteArrayToStringMsg(msg));

        public string SendToID(int ID, byte[] msg)
        {
            var cl = clients.FirstOrDefault(x => x.Id == ID);
            if (cl is null)
                return $"There is no client with id {ID}";

            Send(msg, cl.EndPoint);

            return string.Empty;
        }
        public override void Send(byte[] msg, IPEndPoint host) => Sender.Send(msg, msg.Length, host);
        public void Send(t msg, IPEndPoint endPoint) => base.Send(Helper.ParseObjToSend(msg), endPoint);
        public void BroadCast(t msg) => clients.ForEach( cl => base.Send(Helper.ParseObjToSend(msg), cl.EndPoint));
        private bool IsNewClient(byte[] msg, IPEndPoint endPoint)
        {
            try
            {
                var sMsg = JsonConvert.DeserializeObject<Ramka>(Helper.ByteArrayToStringMsg(msg));

                if (sMsg.msg == "Welcome")
                {
                    var newClient = new Client(endPoint);
                    clients.Add(newClient);
                    OnClientAdded(newClient);
                    Console.WriteLine("New client!!");
                }
                else
                    throw new Exception();

                return true;
            }
            catch
            {
                return false;
            }
        }
        public void DisconnectClient(int id)
        {
            var cl = clients.FirstOrDefault(x => x.Id == id);
            if (cl != null)
                clients.Remove(cl);
        }
            
        public delegate void RecieveHandlerDelegate(t obj, int clientId);
        public RecieveHandlerDelegate RecieveHandler { get; set; }
        public Action<IClient> OnClientAdded { get; set; } = c => { };
    }
}
