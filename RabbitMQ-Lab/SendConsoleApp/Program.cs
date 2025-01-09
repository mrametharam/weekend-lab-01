using RabbitMQ.Client;
using System.Text;

// https://www.rabbitmq.com/tutorials/tutorial-one-dotnet#sending


// Connect to the server. [https://www.rabbitmq.com/client-libraries/dotnet-api-guide#connecting]
var factory = new ConnectionFactory()
{
    HostName = "localhost",
    VirtualHost = "my_vhost",
    UserName = "admin",
    Password = "@dm1n123#",
    AutomaticRecoveryEnabled = true     // https://www.rabbitmq.com/client-libraries/dotnet-api-guide#recovery
};

// https://www.rabbitmq.com/client-libraries/dotnet-api-guide#endpoints-list
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();


// Declare the DLX exchange. [https://www.rabbitmq.com/dlx.html]
await channel.ExchangeDeclareAsync(
    exchange: "dlx",
    type: ExchangeType.Direct);


// Declare queue options
var queueArgs = new Dictionary<string, object?>
{
    { "x-dead-letter-exchange", "dlx" },
    { "x-dead-letter-routing-key", "dlx_routing_key" },
    { "x-message-ttl", 20000 }    // TTL
};

// Declare the queue to connect to. [https://www.rabbitmq.com/client-libraries/dotnet-api-guide#exchanges-and-queues]
await channel.QueueDeclareAsync(
    queue: "hello",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: queueArgs);


// Declare the dead letter queue with TTL and bind it to the DLX exchange. [https://www.rabbitmq.com/dlx.html]
var deadLetterArgs = new Dictionary<string, object?>
{
    { "x-dead-letter-exchange", "dlx" },
    { "x-dead-letter-routing-key", "orig_routing_key" },
    { "x-message-ttl", 15000 }    // TTL
};

// Declare the DLX queue. [https://www.rabbitmq.com/dlx.html]
await channel.QueueDeclareAsync(
    queue: "dead_letter_queue",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: deadLetterArgs
);

await channel.QueueBindAsync(queue: "dead_letter_queue", exchange: "dlx", routingKey: "dlx_routing_key");
await channel.QueueBindAsync(queue: "hello", exchange: "dlx", routingKey: "orig_routing_key");

// Prepare the message.
string message = $"Hello World! {DateTime.UtcNow} -";


// Configure the properties. [https://www.rabbitmq.com/client-libraries/dotnet-api-guide#publishing]
var prop = new BasicProperties()
{
    Expiration = "10000"    // Message expires in 10 seconds.
};

for (int i = 0; i < 10; i++)
{
    var body = Encoding.UTF8.GetBytes($"{message} {i}");


    // Send the message.
    await channel.BasicPublishAsync(
        exchange: string.Empty,
        routingKey: "hello",
        true,
        prop,
        body: body);

    Console.WriteLine($" [x] Sent {message} {i}");
}


Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();