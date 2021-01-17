using Newtonsoft.Json;
using SimpleConnection;
using SimpleConnection.Interfaces;
using SimpleConnection.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SimpleUDP.Server
{
    public class Host<t> : ConnectionHandler
    {
        public List<IClient> clients = new List<IClient>();
        UdpClient Sender;
        bool exit = false;
        public Host(IConnectionHandler connectionHandler) : base(connectionHandler)
        {
            Sender = new UdpClient();
        }

        public void Exit()
        {
            exit = true;
            Sender.Dispose();
        }
        public void Listen()
        {
            while (!exit)
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
        public void Send(t msg, int clinetId)
        {
            var clientEndPoint = clients.First(x => x.Id == clinetId).EndPoint;
            Send(msg, clientEndPoint);
        }
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
                    Console.WriteLine("New client!");
                }

                if (sMsg.msg == "Disconnect")
                {
                    var c = clients.Where(x => x.EndPoint.Equals(endPoint)).FirstOrDefault();
                    if (c != null)
                    {
                        DisconnectClient(c);
                        Console.WriteLine("Client disconnected!");
                    }
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
        public void DisconnectClient(IClient client)
        {
            clients.Remove(client);
            client.ClientRemove();
            OnClientDisconnected(client);
        }
            
        public delegate void RecieveHandlerDelegate(t obj, int clientId);
        public RecieveHandlerDelegate RecieveHandler { get; set; }
        public Action<IClient> OnClientAdded { get; set; } = c => { };
        public Action<IClient> OnClientDisconnected { get; set; } = c => { };
    }
}
