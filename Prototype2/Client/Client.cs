// Define the TCP Client
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

TcpClient client = new TcpClient();

// Connect the client to the Server using defined Address and Port in Constants
try
{
    client.Connect(Constants.Address, Constants.PORT);
    Console.WriteLine("Connected to the server!");
}
catch (Exception ex)
{
    // Handle any other exceptions
    Console.WriteLine("An unexpected error occurred.");
    Console.WriteLine("Exception Message: " + ex.Message);
    Environment.Exit(1); // Exit the program with a non-zero exit code
}

// Define a PacketMaker to encode and read messages
PacketMaker inStream = new PacketMaker();
PacketMaker outStream = new PacketMaker();

// List to store outgoing messages from the client
List<string> outgoingMessages = new List<string>();

// Task to continuously read input from the console and add it to the outgoingMessages list
Task task = new TaskFactory().StartNew(() =>
{
    while (true)
    {
        var messageToSend = Console.ReadLine();
        if (!String.IsNullOrEmpty(messageToSend))
            outgoingMessages.Add(messageToSend);
    }
});

// Main loop to read incoming packets and send outgoing packets 
while (true)
{
    ReadPackets();
    SendPackets();
}

// Function to read incoming packets from the server
void ReadPackets()
{
    var stream = client.GetStream();

    for (int i = 0; i < 10; i++)
    {
        if (stream.DataAvailable)
        {
            byte[] buffer = new byte[client.ReceiveBufferSize];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            (int opcode, string message) = inStream.ParseMessagePacket(buffer.Take(bytesRead).ToArray());

            Console.WriteLine($"Received: [{opcode}] - {message}");
        }
    }
}

// Function to send packets to the server
// We need to define different Opcodes
// 01 : Send and broadcast to other clients
// 02 : Send to server only
// We will change the Opcodes once we have a UI for the client
void SendPackets()
{
    if (outgoingMessages.Count > 0)
    {
        string messageToSend = outgoingMessages[0];

        string pattern = @"(\d+)(\D+)";
        Match match = Regex.Match(messageToSend, pattern);
        int spaceIndex = messageToSend.IndexOf(' ');

        if (match.Success && spaceIndex > 0)
        {
            int opCode = int.Parse(match.Groups[1].Value);
        
            string msgText = messageToSend.Substring(spaceIndex);

            var packet = outStream.CreateMessagePacket(opCode, msgText);
            client.GetStream().Write(packet);
        }
        else
        {
            Console.WriteLine("Please send messages in the format of <OpCode> <Message>");
        }

        outgoingMessages.RemoveAt(0);
    }
}
