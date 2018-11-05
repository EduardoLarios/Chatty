using System;
using System.IO;
using System.Net.Sockets;

namespace Chatty
{
    internal static class Client
    {
        private static TcpClient client;
        private static StreamReader ins;
        private static StreamWriter outs;

        private static void Main()
        {
            // Tries to connect to the server, listening at localhost, port 7777
            try
            {
                // Creates a new TCP Client with IP: localhost and Port: 7777
                client = new TcpClient("0.0.0.0", 7777);
                ins = new StreamReader(client.GetStream());
                outs = new StreamWriter(client.GetStream())
                {
                    // Flushes buffer after every call to StreamWriter.Write()
                    AutoFlush = true
                };
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

    internal class CThread
    {
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
                }
            }

            catch (Exception e)
            {
                // Catches exception and prints its message
                Console.WriteLine(e.Message);
            }

            Environment.Exit(0);
        }
    }
}
