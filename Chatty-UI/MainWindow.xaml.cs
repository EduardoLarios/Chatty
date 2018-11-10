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
            Client.ConnectToServer(ConnectBanner, ConnectButton);
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            Client.ListenForMessages(ConnectBanner, MessageBox, ChatBox);
        }
    }

    static class Client
    {
        private static TcpClient client;
        private static StreamReader ins;
        private static StreamWriter outs;

        // Encapsulates the logic to connect to the server
        public static void ConnectToServer(TextBlock ConnectBanner, Button ConnectButton)
        {
            // Tries to connect to the server, listening at localhost, port 8080
            try
            {
                ConnectBanner.Text = "Looking for server on IP: 127.0.0.1 Port: 8080\n";
                //Console.WriteLine("Looking for server on IP Address: {0}\nPort: {1}", "127.0.0.1", 8080);
                //Console.WriteLine("Trying to connect to the server...\n");

                // Creates a new TCP Client with IP: localhost and Port: 8080 (TCP)
                client = new TcpClient("127.0.0.1", 8080);
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
                //Console.WriteLine("Error: Could not stablish connection to the server");
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }

            // Checks that the client creation was correct and throws and exception in case
            // an error occurs during its creation
            //ListenForMessages();
        }

        // Checks that the client creation was correct and throws and exception in case
        // an error occurs during its creation
        public static void ListenForMessages(TextBlock ConnectBanner, TextBox MessageBox, TextBlock ChatBox)
        {
            if (client != null && outs != null && ins != null)
            {
                try
                {
                    CThread chat = new CThread(client, ins, outs, ChatBox, ConnectBanner);
                    //Thread ctThread = new Thread(chat.Run);
                    //ctThread.Start();
                    chat.Run();

                    // Keeps reading messages from the server while it's open
                    while (!chat.closed)
                    {
                        string msg = MessageBox.Text;
                        //string msg = Console.ReadLine().Trim();
                        outs.WriteLine(msg);
                        MessageBox.Text = "";
                    }

                    // Closes the connection when it receives the signal to close
                    outs.Close();
                    ins.Close();
                    client.Close();
                }

                catch (Exception e)
                {
                    // Catches exception and prints the error to console
                    ConnectBanner.Text = "Error: Could not create client\n";
                    //Console.WriteLine("Error: Could not create client");
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                }
            }
        }
    }

    class CThread
    {
        public bool closed;
        private TcpClient client;
        private StreamReader ins;
        private StreamWriter outs;
        private TextBlock chatBox;
        private TextBlock connectBanner;

        public CThread(TcpClient client, StreamReader ins, StreamWriter outs, TextBlock chatBox, TextBlock connectBanner)
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
                    //chatBox.Dispatcher.Invoke(new Action(() => chatBox.Text += responseLine + "\n"));
                    Console.WriteLine(responseLine);
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
                //connectBanner.Dispatcher.Invoke(new Action(() => connectBanner.Text = "Error: Connection closed unexpectedly"));
                Console.WriteLine("Error: Connection closed unexpectedly");
                Console.WriteLine(e.Message);
            }

            Environment.Exit(0);
        } 
    }
}
