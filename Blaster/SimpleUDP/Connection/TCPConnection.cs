using SimpleConnection.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SimpleConnection.Connection
{
    public class TCPConnection : IConnectionHandler
    {
        TcpListener server { get; set; }
        TcpClient client { get; set; }


        public TCPConnection(IPAddress RemoteIpAnddress, int ServerPort, int RemotePort)
        {
            var localAddr = IPAddress.Parse("127.0.0.1");
            server = new TcpListener(localAddr, ServerPort);
            client = new TcpClient(RemoteIpAnddress.ToString(), RemotePort);
        }
        public void Dispose()
        {
            client.Dispose();
            server.Stop();
        }

        public byte[] Listen(ref IPEndPoint endPoint)
        {
            var bytes = new byte[1024];
            try
            {
                server.Start();     
                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    endPoint = (IPEndPoint)client.Client.RemoteEndPoint;
                    NetworkStream stream = client.GetStream();
                    stream.Read(bytes, 0, bytes.Length);
                    client.Close();
                    return bytes;
                }            
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

            finally
            {
                server.Stop();             
            }
            return bytes;
        }

        public void Send(byte[] msg, IPEndPoint host)
        {    
            var stream = client.GetStream();
            stream.Write(msg, 0, msg.Length);
        }
    }
}
