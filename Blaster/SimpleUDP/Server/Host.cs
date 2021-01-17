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
using System.Threading.Tasks;

namespace SimpleUDP.Server
{
    public class Host<t> : ConnectionHandler
    {
        public List<IClient> clients = new List<IClient>();
        Dictionary<string,(t, IPEndPoint)> framesToResend = new Dictionary<string, (t, IPEndPoint)>();

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
                {
                    var client = clients.FirstOrDefault(x => x.EndPoint.Equals(endPoint));
                    if (client != null)
                    {
                        var _msg = DeParseCommand(msg);

                        RecieveHandler(_msg, client.Id);
                    }
                }
            }
        }

        private t DeParseCommand(byte[] msg) {

            var strMag = Helper.ByteArrayToStringMsg(msg);
            var r = Helper.EncapsulateFromFrame(strMag);
            if (!string.IsNullOrEmpty(r.ackHash))
                RemoveFrameFromStack(r.ackHash);

            var m = JsonConvert.DeserializeObject<t>(r.msg);
            return m;
        }

        private void RemoveFrameFromStack(string ackHash)
        {
            lock (framesToResend)
            {
                framesToResend.Remove(ackHash);
            }
        }

        public string SendToID(int ID, byte[] msg)
        {
            var cl = clients.FirstOrDefault(x => x.Id == ID);
            if (cl is null)
                return $"There is no client with id {ID}";

            Send(msg, cl.EndPoint);

            return string.Empty;
        }
        private void Send(t msg, IPEndPoint endPoint, bool forceResponse = false)
        {
            var ackHash = string.Empty;

            if (forceResponse) {
                ackHash = Helper.Generatehash();
                lock (framesToResend)
                {
                    framesToResend.Add(ackHash, (msg, endPoint) );
                }

                new TaskFactory().StartNew(
                    () =>
                    {
                        Task.Delay(1000);
                        lock (framesToResend)
                        {
                            if (framesToResend.Any())
                            {
                                foreach (var toSend in framesToResend)
                                {
                                    Send(toSend.Value.Item1, toSend.Value.Item2, true);
                                }
                            }
                        }
                    }
                    );
            }

            base.Send(Helper.ParseObjToSend(msg, ackHash), endPoint); 
        }

        public void Send(t msg, int clinetId, bool forceResponse = false)
        {
            var clientEndPoint = clients.First(x => x.Id == clinetId).EndPoint;
            Send(msg, clientEndPoint);
        }
        public void BroadCast(t msg, bool forceResponse = false) => clients.ForEach( cl => Send(msg, cl.EndPoint, forceResponse));
        private bool IsNewClient(byte[] msg, IPEndPoint endPoint)
        {
            try
            {
                var sMsg = JsonConvert.DeserializeObject<Ramka>(JsonConvert.DeserializeObject<Ramka>(Helper.ByteArrayToStringMsg(msg)).msg);

                if (sMsg.msg == "Welcome")
                {
                    var newClient = new Client(endPoint);
                    clients.Add(newClient);
                    OnClientAdded(newClient);
                    Console.WriteLine("New client!");

                    return true;
                }

                else if (sMsg.msg == "Disconnect")
                {
                    var c = clients.Where(x => x.EndPoint.Equals(endPoint)).FirstOrDefault();
                    if (c != null)
                    {
                        DisconnectClient(c);
                        Console.WriteLine("Client disconnected!");
                    }
                }

                return false;
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
