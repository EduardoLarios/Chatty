using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;

namespace Chatty
{
    class Server
    {
        private static TcpListener serverSocket = default(TcpListener);
        private static Socket clientSocket = default(Socket);

        // Sets the max number of clients that can be connected at
        // any given time
        private const int maxClientConnections = 20;
        private static readonly ClientHandle[] clients = new ClientHandle[maxClientConnections];

        static void Main()
        {
            var ipServer = IPAddress.Any;
            var serverPort = 8080;
            // Sets the server address to 127.0.0.1 (localhost) and port to 8080 (TCP) and starts it
            serverSocket = new TcpListener(ipServer, serverPort);
            clientSocket = default(Socket);
            serverSocket.Start();

            while (true)
            {
                // Listens for incoming connections via sockets
                Console.WriteLine("Listening on IP Address: {0}\nPort: {1}", ipServer, serverPort);
                Console.WriteLine("Waiting for connections...");

                // Accepts socket and alerts of its connection
                clientSocket = serverSocket.AcceptSocket();
                Console.WriteLine("Connection stablished");
                int i = 0;

                // Checks for each connection if it's a valid client
                // otherwise creates a new ClientHandle thread for it
                for (i = 0; i < maxClientConnections; i++)
                {
                    if (clients[i] == null)
                    {
                        (clients[i] = new ClientHandle()).StartClient(clientSocket, clients);
                        break;
                    }
                }

                // If the server is at max capacity it closes the incoming socket and
                // won't accept any more incoming connections
                if (i == maxClientConnections)
                {
                    StreamWriter outs = new StreamWriter(new NetworkStream(clientSocket));
                    outs.AutoFlush = true;

                    // Alerts that the server is full
                    outs.WriteLine("The server can't handle any more connections");
                    outs.Close();
                    clientSocket.Close();
                }
            }
        }
    }

    // Handles a client's connection and its messages
    // Each client is its own thread
    public class ClientHandle
    {
        private Socket clientSocket;
        private ClientHandle[] clients;
        private int maxClientConnections;
        private string clientName;
        private StreamReader ins;
        private StreamWriter outs;

        // Initializes client and starts its thread
        public void StartClient(Socket inClientSocket, ClientHandle[] clients)
        {
            clientSocket = inClientSocket;
            this.clients = clients;
            maxClientConnections = clients.Length;

            // Instanciates the thread
            Thread chatThread = new Thread(CreateChat);
            chatThread.Start();
        }

        // Checks if the message is valid i.e no white spaces or not alphanumeric
        private bool CheckValid(string msg)
        {
            if (msg.Equals("") || msg.Equals("\n"))
            {
                return false;
            }

            foreach (var letter in msg)
            {
                if (!char.IsLetterOrDigit(letter))
                {
                    return false;
                }
            }

            return true;
        }

        // Checks if the message is a valid command i.e /list or /quit
        private bool IsCommand(string msg)
        {
            return msg.Equals("/list") || msg.Equals("/quit") || msg.Equals("") || msg.Equals("\n");
        }

        // Creates a new chat instance
        private void CreateChat()
        {
            // This method is created for every client thread to manage its messages
            int maxClientConnections = this.maxClientConnections;
            var clients = this.clients;

            // Tries to open connection with a client socket
            try
            {
                // Opens connection with a client socket
                ins = new StreamReader(new NetworkStream(clientSocket));
                outs = new StreamWriter(new NetworkStream(clientSocket))
                // Flushes buffer after every call to StreamWriter.Write()
                {
                    AutoFlush = true
                };

                string name;

                // Asks for a name and checks its validity
                do
                {
                    outs.WriteLine("Please write a username: ");
                    name = ins.ReadLine().Trim();

                    // If the name is valid breaks out of the cycle, otherwise it keeps
                    // asking for a valid input
                    if (CheckValid(name)) { break; }
                    else
                    {
                        outs.WriteLine("Invalid name: Must not contain special characters");
                        name = null;
                    }

                } while (true);

                // The server sends a welcome message to the user
                Console.WriteLine("User: {0} connected successfully", name);
                // Explains to the user the valid commands
                outs.WriteLine("\nWelcome {0}\nTo exit type: /quit on a new line", name);
                outs.WriteLine("To list connected users type: /list");

                // The lock is used to synchronize the statement by holding the object (ClientHandle) blocked 
                // until the next statements are complete
                lock (this)
                {
                    // Formats the client name to look more "natural"
                    foreach (var client in clients)
                    {
                        if (client != null && client == this)
                        {
                            clientName = "@" + name;
                            break;
                        }

                    }

                    // Notifies the rest of the clients of the new user
                    foreach (var client in clients)
                    {
                        client.outs.WriteLine("New user connected: {0}", name);
                    }
                }

                // Handles messages incoming from the client
                while (true)
                {
                    // Checks if the message is invalid or a command
                    string line = ins.ReadLine();
                    if (line.StartsWith("/quit")) break;

                    // Lists all connected clients
                    if (line.StartsWith("/list"))
                    {
                        foreach (var client in clients)
                        {
                            if (client != null && client != this)
                                outs.WriteLine(client.clientName);
                        }
                    }

                    // Validates message minimum lenght
                    if (line.Length < 2) outs.WriteLine("Message is too short");

                    // TODO: Add comments explaining this section
                    if (line.StartsWith("@"))
                    {
                        var words = Regex.Split(line, "\\s");
                        if (words.Length > 1 && words[1] != null)
                        {
                            words[1] = words[1].Trim();
                            if (words[1].Length > 0)
                            {
                                lock (this)
                                {
                                    foreach (var client in clients)
                                    {
                                        if (client != null && client != this && client.clientName?.Equals(words[0]) == true)
                                        {
                                            client.outs.WriteLine("< {0} > {1}", name, words[1]);
                                            outs.WriteLine("> {0} > {1}", name, words[1]);
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                    }

                    else
                    {
                        lock (this)
                        {
                            if (!IsCommand(line))
                            {
                                foreach (var client in clients)
                                {
                                    if (client?.clientName != null)
                                        client.outs.WriteLine("< {0} > {1}", name, line);
                                }

                            }
                        }

                    }
                }

                // User disconnected notification
                Console.WriteLine("User: {0} disconnected from the lobby", name);
                lock (this)
                {
                    // Notifies clients of user disconnection
                    foreach (var client in clients)
                    {
                        if (client != null)
                            client.outs.WriteLine("User {0} has disconnected", name);
                    }
                }

                // Bids farewell to the client
                outs.WriteLine("Goodbye {0}", name);

                // Clears the client that disconnected from the clients list
                // Requires a lock to synchronize all the clients' states
                lock (this)
                {
                    for (int i = 0; i < maxClientConnections; i++)
                    {
                        if (clients[i] == this)
                        {
                            clients[i] = null;
                        }
                    }
                }

                // Closes the socket to the client
                ins.Close();
                outs.Close();
                clientSocket.Close();

            }

            // Catches the failure to connect to a client
            catch (Exception e)
            {
                Console.WriteLine("Error: Could not stablish connection to the client");
                Console.WriteLine(e.Message);
            }
        }
    }
}
    