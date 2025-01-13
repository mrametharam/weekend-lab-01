using RabbitMQ.Client;
using System.Text;

// https://www.rabbitmq.com/tutorials/tutorial-three-dotnet#publishsubscribe


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


// Define the exchange. [https://www.rabbitmq.com/tutorials/tutorial-three-dotnet#exchanges]
await channel.ExchangeDeclareAsync(
    exchange: "logs",
    type: ExchangeType.Fanout);


// Prepare the message.
string message = $"Hello World! {DateTime.UtcNow} -";


// Configure the properties. [https://www.rabbitmq.com/client-libraries/dotnet-api-guide#publishing]
var prop = new BasicProperties()
{
    Expiration = "10000",    // Message expires in 10 seconds.
    Persistent = true
};


for (int i = 0; i < 500_000; i++)
{
    var body = Encoding.UTF8.GetBytes($"{message} {i}");


    // Send the message.
    await channel.BasicPublishAsync(
        exchange: "logs",
        routingKey: string.Empty,
        true,
        prop,
        body: body);

    Console.WriteLine($" [x] Sent {message} {i}");
}


Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();
