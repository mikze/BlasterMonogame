using SimpleConnection.Client;
using SimpleConnection.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Blaster.Network.NetworkHelper;

namespace Blaster.Network
{
    public static class BlasterClient
    {
        static Client<Frame> client;
        static IPEndPoint endPoint;
        static List<Frame> frames = new List<Frame>();
        static bool listening = true;
        static CancellationTokenSource tokenSource2;
        static CancellationToken ct;

        public static void Connect(IPEndPoint EndPoint)
        {
            client = new Client<Frame>(new UDPConnection(0));
            client.Connect(EndPoint);
            endPoint = EndPoint;
            StartListening();
        }

        static void Listen()
        {
            tokenSource2 = new CancellationTokenSource();
            ct = tokenSource2.Token;

            new TaskFactory().StartNew(
                () =>
                    {
                        while (listening)
                        {
                            var f = client.Listen();
                            lock (frames)
                            {
                                frames.Add(f);
                            }
                        }
                    }
                , tokenSource2.Token);
        }

        private static void StopListening() { tokenSource2.Cancel(); listening = false; }

        public static void Disconnect()
        {
            StopListening();
            client.Disconnect(endPoint);          
        }

        public static void StartListening()
        {
            listening = true;
            Listen();
        }

        public static void Send(Frame frame) => client.Send(frame, endPoint);

        public static Frame[]  GetFrames()
        {
            lock (frames)
            {
                var toReturn = frames.ToArray();
                frames.Clear();
                return toReturn;
            }
        }

        public static Frame[] GetFrames(int id)
        {
            lock (frames)
            {
                var toReturn = frames.Where(x => x.id == id || (FrameKind)x.FrameKind == FrameKind.newPLayer).ToArray();
                foreach (var f in toReturn)
                    frames.Remove(f);

                return toReturn;
            }
        }
    }
}
