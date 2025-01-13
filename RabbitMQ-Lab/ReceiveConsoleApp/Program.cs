using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

// https://www.rabbitmq.com/tutorials/tutorial-one-dotnet#receiving


// Connect to the server. [https://www.rabbitmq.com/client-libraries/dotnet-api-guide#connecting]
var factory = new ConnectionFactory()
{
    HostName = "rabbitmq",      //"10.88.0.8",  //"localhost",
    VirtualHost = "my_vhost",
    UserName = "admin",
    Password = "@dm1n123#",
    AutomaticRecoveryEnabled = true
};

using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

// Declare queue options
var queueArgs = new Dictionary<string, object?>
{
    { "x-dead-letter-exchange", "dlx" },
    { "x-dead-letter-routing-key", "dlx_routing_key" },
    { "x-message-ttl", 20000 }    // TTL is 20 seconds.
};

// Declare the queue to connect to.
await channel.QueueDeclareAsync(
    queue: "hello",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: queueArgs);

Console.WriteLine(" [*] Waiting for messages.");


// Create a consumer.
var consumer = new AsyncEventingBasicConsumer(channel);

int processed = 0;

// Subscribe to the ReceivedAsync event.
consumer.ReceivedAsync += async (model, ea) =>
{
    processed++;

    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);

    if ((processed % 5) == 0)
    {
        await channel.BasicNackAsync(
            deliveryTag: ea.DeliveryTag,
            multiple: false,
            requeue: false);

        Console.WriteLine($" [x] Rejected {message}");

        return;
    }

    Console.WriteLine($" [x] Received {message}");

    await channel.BasicAckAsync(
        deliveryTag: ea.DeliveryTag,
        multiple: false);
};


await channel.BasicConsumeAsync(
    queue: "hello",
    autoAck: false,
    consumer: consumer);


Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();
