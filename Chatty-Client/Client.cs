using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Chatty
{
    class Client
    {
        private static TcpClient client;
        private static StreamReader ins;
        private static StreamWriter outs;

        static void Main()
        {
            // Tries to connect to the server, listening at localhost, port 8080
            try
            {
                var clientAddress = "127.0.0.1";
                var clientPort = 8080;
                Console.WriteLine("Looking for server on IP Address: {0}\nPort: {1}", clientAddress, clientPort);
                Console.WriteLine("Trying to connect to the server...\n");

                // Creates a new TCP Client with IP: localhost and Port: 8080 (TCP)
                client = new TcpClient(clientAddress, clientPort);
                ins = new StreamReader(client.GetStream());
                outs = new StreamWriter(client.GetStream());
                // Flushes buffer after every call to StreamWriter.Write()
                outs.AutoFlush = true;
            }

            // Catches exception if the connection to the server fails
            catch (Exception e)
            {
                Console.WriteLine("Error: Could not stablish connection to the server");
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }

            // Checks that the client creation was correct and throws and exception in case
            // an error occurs during its creation
            if (client != null && outs != null && ins != null)
            {
                try
                {
                    var cli = new CThread(client, ins, outs);
                    Thread chatThread = new Thread(cli.Run);
                    chatThread.Start();

                    while (!cli.closed)
                    {
                        string msg = Console.ReadLine().Trim();
                        outs.WriteLine(msg);
                    }

                    outs.Close();
                    ins.Close();
                    client.Close();
                }

                catch (Exception e)
                {
                    // Catches exception and prints the error to console
                    Console.WriteLine("Error: Could not create client");
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                }
            }
        }
    }

    class CThread
    {
        public bool closed = false;
        private TcpClient client;
        private StreamReader ins;
        private StreamWriter outs;

        public CThread(TcpClient client, StreamReader ins, StreamWriter outs)
        {
            this.client = client;
            this.ins = ins;
            this.outs = outs;
        }

        public void Run()
        {
            string response;
            try
            {
                // Listens for the server to terminate the connection
                while ((response = ins.ReadLine()) != null)
                {
                    Console.WriteLine(response);
                    if (response.Contains("Goodbye"))
                    {
                        break;
                    }

                    //if (response.IndexOf("Goodbye") != -1)
                    //{
                    //    break;
                    //}
                }

                closed = true;
            }

            catch (Exception e)
            {
                // Catches exception and prints its message
                Console.WriteLine("Error: Connection closed unexpectedly");
                Console.WriteLine(e.Message);
            }

            Environment.Exit(0);
        }
    }
}