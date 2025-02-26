

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory factory = new();
factory.Uri = new("amqps://vlacgcyu:Ph-9ekMyFDyZ9LrXeHD55ZJjD9ixehJT@woodpecker.rmq.cloudamqp.com/vlacgcyu");
using IConnection connection = await factory.CreateConnectionAsync();
using IChannel channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(exchange: "fanout-exchange-example",type:ExchangeType.Fanout);

Console.Write("Queue Name:");
string? quequeName = Console.ReadLine();
await channel.QueueDeclareAsync(queue:quequeName,exclusive:false);

await channel.QueueBindAsync(queue:quequeName,exchange: "fanout-exchange-example",routingKey:string.Empty);

AsyncEventingBasicConsumer consumer = new(channel);

await channel.BasicConsumeAsync(queue:quequeName,autoAck:true,consumer:consumer);

consumer.ReceivedAsync += async (sender, e) =>
{
    Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
};

Console.Read();