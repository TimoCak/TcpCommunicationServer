// See https://aka.ms/new-console-template for more information


using System.IO;
using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Threading;

namespace NetworkTcpCommunication { 
    public class MainClass
    {
        public static void Main(string[] args)
        {
            TcpServer server = new(8888, "192.168.178.63");
                
            server.StartServer();
        }
    }

    public class TcpServer
    {
        public Int32 port;
        public IPAddress ip;
        public TcpListener server = null;

        public TcpServer(Int32 port, string ip)
        {
            this.port = port;
            this.ip = IPAddress.Parse(ip);
        }

        public void StartServer()
        {
            try
            {
                this.server = new TcpListener(ip, port);

                this.server.Start();

                //listening loop
                while (true)
                {
                    TcpClient client = this.server.AcceptTcpClient();

                    var childSocketThread = new Thread(() =>
                    {
                        Console.WriteLine("IP-Address: {0}", client.Client.RemoteEndPoint.ToString());
                        AcceptClientConnection(client);
                    });

                    childSocketThread.Start();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                this.server.Stop();
            }

        }

        public static void AcceptClientConnection(TcpClient client)
        {
            //loop to receive all the data sent by the client
            Byte[] buffer = new Byte[1024];
            string data = "";

            Console.WriteLine("Waiting for a connection...");

            Console.WriteLine("Connected");

            NetworkStream stream = client.GetStream();

            int i;

            //loop to receive all the data sent by the client
            while ((i = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                data = System.Text.Encoding.ASCII.GetString(buffer, 0, i);
                Console.WriteLine("Received: {0}", data);
                string command = data.Replace("\n", "").Replace("\r", "");
                if (command=="time")
                {
                    DateTime time = DateTime.Now;
                    data = time.ToString();
                } 
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);
                stream.Write(msg, 0, msg.Length);
                Console.WriteLine("Sent: {0}", data);
            }
        }
    }
}
