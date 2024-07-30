using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

internal class Program
{
    private static void Main(string[] args)
    {
        // Create a channel for receiving messages
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
        Console.WriteLine(" Server: Waiting for messages.");    // channel ready

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($" Server: Received {message}");
        };

        channel.BasicConsume(queue: "channel1",
                             autoAck: true,
                             consumer: consumer);

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
}
