﻿using log4net;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using universal_blockchain.Encrypt;

namespace universal_blockchain.Server
{
    class TcpServer
    {
        private TcpListener _server;
        private Boolean _isRunning;
        private static ILog log { get; set; }

        public TcpServer(int port,IPAddress ip)
        {
            _server = new TcpListener(ip, port);
            _server.Start();
            log = Configuration.GetLogger();
            _isRunning = true;
            log.Info("Server started on IP: " + ip.ToString() + ":" + port.ToString());
        }

        public void LoopClients()
        {
            while (_isRunning)
            {
                // wait for client connection
                TcpClient newClient = _server.AcceptTcpClient();

                // client found.
                // create a thread to handle communication
                Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
                t.Start(newClient);
            }
        }

        public void HandleClient(object obj)
        {
            // retrieve client from parameter passed to thread
            TcpClient client = (TcpClient)obj;

            // sets two streams
            StreamWriter sWriter = new StreamWriter(client.GetStream(), Encoding.UTF8);
            StreamReader sReader = new StreamReader(client.GetStream(), Encoding.UTF8);
            // you could use the NetworkStream to read and write, 
            // but there is no forcing flush, even when requested

            Boolean bClientConnected = true;
            String sData = null;
            log.Info("Client connected: "+ ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
            while (bClientConnected)
            {
                // reads from stream
                sData = TextEncryptor.Decrypt(sReader.ReadLine(),Settings.node.node_encrypt_key);

                // shows content on the console.
                log.Info("(Server)Client data: " + sData);
                Console.WriteLine("(Server)Client data: " + sData);

                if (sData == "exit")
                {
                    break;
                }

                // to write something back.
                sWriter.WriteLine(sData);
                sWriter.Flush();
            }
            sWriter.Close();
            sReader.Close();

        }

        public static IPAddress GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
    
}