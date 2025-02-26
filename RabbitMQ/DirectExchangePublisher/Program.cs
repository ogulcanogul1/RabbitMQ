
using RabbitMQ.Client;
using System.Text;

ConnectionFactory factory = new();
factory.Uri = new("amqps://vlacgcyu:Ph-9ekMyFDyZ9LrXeHD55ZJjD9ixehJT@woodpecker.rmq.cloudamqp.com/vlacgcyu");

using IConnection connection = await factory.CreateConnectionAsync();
using IChannel channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(exchange:"direct-exchange-example",type:ExchangeType.Direct);


while (true)
{
    Console.Write("Mesaj:");
    string? message = Console.ReadLine();
    byte[] byteMessage = Encoding.UTF8.GetBytes(message);

    await channel.BasicPublishAsync(
        exchange:"direct-exchange-example",
        routingKey:"direct-queue-example",
        body:byteMessage);
}