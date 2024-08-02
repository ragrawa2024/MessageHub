using System;

namespace Tests;

public class UnitTest1
{
    [Fact]
    public void TestPacketMaker()
    {
        var inStream = new PacketMaker();
        byte[] byteArray = inStream.CreateMessagePacket(10, "Hello");

        (int opCode, string message) = inStream.ParseMessagePacket(byteArray);

        Assert.True(opCode == 10);
        Assert.True(message == "Hello");
    }
}
