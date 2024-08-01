using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

public class Constants
{
    //Choose a port that's not already occupied.
    public static int PORT = 5000;

    //Use localhost since the system has to work locally
    public static IPAddress Address = IPAddress.Loopback;

    // Max number of clients allowed
    public static int MAXCLIENTS = 2;
}

public class MessagePacket
{
    public DateTime TimeStamp { get; set;}
    public int clientID { get; set; }
    public int opCode { get; set; }
    public string msg { get; set; }
}

public class PacketMaker
{
    // Function to create a message packet
    // This function takes in an opcode (operation code) and a message,
    // and generates a byte array. This byte array can be directly sent over the network.
    public byte[] CreateMessagePacket(int opcode, string message)
    {
        // Use a memory stream to hold the data
        // BinaryWriter is then used to write the opcode and the message to this stream
        using MemoryStream ms = new MemoryStream();
        using BinaryWriter writer = new BinaryWriter(ms);

        writer.Write(opcode);     // Write opcode into the stream
        writer.Write(message);    // Write message into the stream
        
        // Get the byte array to send over the network
        return ms.ToArray();
    }

    // Function to parse incoming data into opcode and message
    // This function takes in a byte array, which is the format as created by the
    // CreateMessagePacket function, and extracts the opcode and the message from it
    public (int opcode, string message) ParseMessagePacket(byte[] data)
    {
        // Use a memory stream to hold the data
        // BinaryReader is then used to read the opcode and the message from this stream
        using MemoryStream ms = new MemoryStream(data);
        using BinaryReader reader = new BinaryReader(ms, Encoding.ASCII);

        int opcode = reader.ReadInt32();        // Read opcode from the stream
        string message = reader.ReadString();   // Read message from the stream
        
        // Return the opcode and the message
        return (opcode, message);
    }
}