using System;
using System.IO;
using System.Windows;
using System.Threading;
using System.Net.Sockets;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Chatty_UI
{
    // Interaction logic for MainWindow.xaml
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Start_Client(object sender, RoutedEventArgs e)
        {
            Client.ConnectToServer(ConnectBanner, ConnectButton, SendButton);
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            SendButton.Content = "Send";
            ConnectBanner.Text = "Connected to Server";
            Client.ListenForMessages(ConnectBanner, MessageBox, ChatBox);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Client.CallExit(ConnectBanner, ChatBox);
        }

        private void List_Click(object sender, RoutedEventArgs e)
        {
            Client.CallList(ConnectBanner, ChatBox);
        }

        private void EnterSend(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Send_Click(sender, e);
            }

        }

        private void ClearChat(object sender, RoutedEventArgs e)
        {
            ChatBox.Text = "";
        }
    }

    static class Client
    {
        private static TcpClient client;
        private static StreamReader ins;
        private static StreamWriter outs;

        // Encapsulates the logic to connect to the server
        public static void ConnectToServer(TextBlock ConnectBanner, Button ConnectButton, Button SendButton)
        {
            // Tries to connect to the server, listening at localhost, port 8080
            try
            {
                ConnectBanner.Text = "Looking for server on IP: 127.0.0.1 Port: 8080\n";
                //Console.WriteLine("Looking for server on IP Address: {0}\nPort: {1}", "127.0.0.1", 8080);
                //Console.WriteLine("Trying to connect to the server...\n");

                // Creates a new TCP Client with IP: localhost and Port: 8080 (TCP)
                client = new TcpClient("10.25.70.62", 8080);
                ins = new StreamReader(client.GetStream());
                outs = new StreamWriter(client.GetStream());
                // Flushes buffer after every call to StreamWriter.Write()
                outs.AutoFlush = true;
            }

            // Catches exception if the connection to the server fails
            catch (Exception e)
            {
                ConnectBanner.Text += "Error: Could not stablish connection to the server";
                ConnectButton.Content = "Try Again";

                Console.WriteLine("Error: Could not stablish connection to the server");
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }

            ConnectBanner.Text = "Please type your username in the chatbox: ";
            SendButton.Content = "Confirm";
        }

        // Checks that the client creation was correct and throws and exception in case
        // an error occurs during its creation
        public static void ListenForMessages(TextBlock ConnectBanner, TextBox MessageBox, TextBlock ChatBox)
        {
            if (client != null && outs != null && ins != null)
            {
                try
                {
                    ChatThread chat = new ChatThread(client, ins, outs, ChatBox, ConnectBanner);
                    Thread ctThread = new Thread(chat.Run);
                    ctThread.Start();

                    // Keeps reading messages from the server while it's open
                    if (!chat.closed)
                    {
                        string msg = MessageBox.Text;
                        outs.WriteLine(msg);
                        MessageBox.Text = "";
                    }
                }

                catch (Exception e)
                {
                    // Catches exception and prints the error to console
                    ConnectBanner.Text = "Error: Could not create client\n";

                    Console.WriteLine("Error: Could not create client");
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                }
            }
        }

        public static void CallExit(TextBlock ConnectBanner, TextBlock ChatBox)
        {
            if (client != null && outs != null && ins != null)
            {
                try
                {
                    ChatThread chat = new ChatThread(client, ins, outs, ChatBox, ConnectBanner);
                    Thread ctThread = new Thread(chat.Run);
                    ctThread.Start();

                    // Keeps reading messages from the server while it's open
                    if (!chat.closed)
                    {
                        const string msg = "/quit";
                        //string msg = Console.ReadLine().Trim();
                        outs.WriteLine(msg);
                    }
                }

                catch (Exception e)
                {
                    // Catches exception and prints the error to console
                    ConnectBanner.Text = "Error: Could not create client\n";

                    Console.WriteLine("Error: Could not create client");
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                }
            }
        }

        public static void CallList(TextBlock ConnectBanner, TextBlock ChatBox)
        {
            if (client != null && outs != null && ins != null)
            {
                try
                {
                    ChatThread chat = new ChatThread(client, ins, outs, ChatBox, ConnectBanner);
                    Thread ctThread = new Thread(chat.Run);
                    ctThread.Start();

                    // Keeps reading messages from the server while it's open
                    if (!chat.closed)
                    {
                        const string msg = "/list";
                        //string msg = Console.ReadLine().Trim();
                        outs.WriteLine(msg);
                    }
                }

                catch (Exception e)
                {
                    // Catches exception and prints the error to console
                    ConnectBanner.Text = "Error: Could not create client\n";

                    Console.WriteLine("Error: Could not create client");
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                }
            }
        }
    }

    class ChatThread
    {
        public bool closed;
        private TcpClient client;
        private StreamReader ins;
        private StreamWriter outs;
        private TextBlock chatBox;
        private TextBlock connectBanner;

        public ChatThread(TcpClient client, StreamReader ins, StreamWriter outs, TextBlock chatBox, TextBlock connectBanner)
        {
            this.client = client;
            this.ins = ins;
            this.outs = outs;
            this.chatBox = chatBox;
            this.connectBanner = connectBanner;
        }

        public void Run()
        {
            string responseLine;
            try
            {
                // Listens for the server to terminate the connection
                while ((responseLine = ins.ReadLine()) != null)
                {
                    chatBox.Dispatcher.Invoke(new Action(() => chatBox.Text += "  " + responseLine + "\n"));
                    if (responseLine.Contains("Goodbye"))
                    {
                        break;
                    }
                }

                closed = true;
            }

            catch (Exception e)
            {
                // Catches exception and prints its message
                connectBanner.Dispatcher.Invoke(new Action(() => connectBanner.Text = "Error: Connection closed unexpectedly"));
                Console.WriteLine("Error: Connection closed unexpectedly");
                Console.WriteLine(e.Message);
            }

            Environment.Exit(0);
        }
    }
}
