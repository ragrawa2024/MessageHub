using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.Json;

internal class Program
{
    static Dictionary<int, TcpClient> _clients = new Dictionary<int, TcpClient>();
    static int nextClientId = 1;

    private static void Main(string[] args)
    {
        // Messenger instances for data sending and receiving
        var inStream = new PacketMaker();
        var outStream = new PacketMaker();

        // A list to keep track of all connected clients
        //List<TcpClient> _clients = new List<TcpClient>();

        // TCP listener for accepting client connections
        TcpListener listener = new TcpListener(Constants.Address, Constants.PORT);
        listener.Start();
        Console.WriteLine("Server started at " + Constants.Address.ToString() + " Port: " + Constants.PORT.ToString());

        // Logfile to persist all cllient messages sent to the server
        const string filePath = @"MessageLog.json";
        CreateLogFile(filePath);

        // Task to continuously read input from the console and broadcast to all
        new TaskFactory().StartNew(() =>
        {
            while (true)
            {
                var messageToSend = Console.ReadLine();
                if (!String.IsNullOrEmpty(messageToSend))
                {
                    Broadcast(messageToSend);
                }
                else
                {
                    Broadcast("Server shutting down ....");
                    listener.Stop();
                    Environment.Exit(0);
                }
            }
        });

        while (true)
        {
            // Accept client connections
            if (_clients.Count() < Constants.MAXCLIENTS)
                AcceptClients();

            // Receive message from each connected client
            ReceiveMessage();
        }

        // Function to accept client connections
        void AcceptClients()
        {
            for (int i = 0; i < 5; i++)
            {
                if (!listener.Pending())
                    continue;

                // If a client connection is pending, accept it and add to the _clients list
                var client = listener.AcceptTcpClient();

                int clientId = nextClientId++;
                _clients.Add(clientId, client);
                Console.WriteLine("Client {0} connected from {1}.", clientId, client.Client.RemoteEndPoint);

                Broadcast($"We now have {clientId} clients.");  // inform everyone
            }
        }

        // Function to receive message from each client
        void ReceiveMessage()
        {
            foreach (var client in _clients)
            {
                NetworkStream stream = client.Value.GetStream();

                if (stream.DataAvailable)
                {
                    // Read data from the client and parse it into a message packet
                    byte[] buffer = new byte[client.Value.ReceiveBufferSize];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    (int opCode, string message) = inStream.ParseMessagePacket(buffer.Take(bytesRead).ToArray());

                    Console.WriteLine($"Received: [{opCode}] - {message}");

                    // Serialize the message to the log file
                    MessagePacket mPacket = new MessagePacket();
                    mPacket.clientID = client.Key;
                    mPacket.TimeStamp = DateTime.Now;
                    mPacket.opCode = opCode;
                    mPacket.msg = message;

                    string json = JsonSerializer.Serialize(mPacket);
                    File.AppendAllText(filePath, json + Environment.NewLine);
                    File.AppendText("\n\n");

                    // Broadcast received message to all other connected clients depending on the opCode
                    if (opCode == 1)
                        Broadcast(message, client.Value);
                }
            }
        }

        // Function to broadcast a message from one client to all other connected clients
        // if sender == null, then Broadcast to all.
        void Broadcast(string message, TcpClient? sender = null)
        {
            foreach (var client in _clients.Values.Where(x => x != sender))    // skip sending the message back to the sender
            {
                // Create a message packet and send it to the connected clients
                int opCode = 10; // currently hardcoded to imply message from Server
                var packet = outStream.CreateMessagePacket(opCode, message);

                NetworkStream stream = client.GetStream();
                stream.Write(packet);

                //byte[] byteArray = Encoding.ASCII.GetBytes(message);
                //stream.Write(byteArray, 0, message.Length);   // does not work since the Client parses for opcode.
            }
        }

        void CreateLogFile(string filePath)
        {
            // Logfile to keep track of messages

            try
            {
                // Check if the file exists
                if (File.Exists(filePath))
                {
                    // Delete the file
                    File.Delete(filePath);
                    Console.WriteLine("Log file deleted successfully.");
                }

                // Create a new file and write text to it
                using (StreamWriter sw = File.CreateText(filePath))
                {
                    sw.WriteLine("New log started at " + DateTime.Now);
                    Console.WriteLine("New log file created.");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur
                Console.WriteLine("An error occurred while creating the file: " + ex.Message);
            }

        }
    }
}
