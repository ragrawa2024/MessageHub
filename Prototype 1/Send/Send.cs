using System.Text;
using RabbitMQ.Client;

internal class Program
{
    private static void Main(string[] args)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = factory.CreateConnection();

        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: "channel1",
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        Console.Clear();

        DateTime dat = DateTime.Now;

        Console.WriteLine("\nToday is {0:d} at {0:T}.", dat);
        Console.WriteLine("Client ready ... Type anything to send. Press [enter] to exit");

        string message = Console.ReadLine();
        while (!String.IsNullOrEmpty(message)) {
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: string.Empty,
                                routingKey: "channel1",
                                basicProperties: null,
                                body: body);
            Console.WriteLine($"[Client]: Sent {message}");
            message = Console.ReadLine();
        }

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
}
