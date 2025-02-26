

using RabbitMQ.Client;
using System.Text;

ConnectionFactory factory = new();

factory.Uri = new("amqps://vlacgcyu:Ph-9ekMyFDyZ9LrXeHD55ZJjD9ixehJT@woodpecker.rmq.cloudamqp.com/vlacgcyu");

using IConnection connection = await factory.CreateConnectionAsync();
using IChannel channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(exchange: "topic-exchange-example", type: ExchangeType.Topic);


for (int i = 0; i < 100; i++)
{
    byte[] byteMessage = Encoding.UTF8.GetBytes($"Merhaba {i}");
    Console.Write("Mesajın Gönderileceği topic formatını belirtiniz : ");
    string? topic = Console.ReadLine();
    await channel.BasicPublishAsync(
        exchange: "topic-exchange-example" , 
        routingKey:topic,
        body:byteMessage);
};

Console.Read();
