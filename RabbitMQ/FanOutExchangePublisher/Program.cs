
using RabbitMQ.Client;
using System.Text;

ConnectionFactory factory = new();
factory.Uri = new("amqps://vlacgcyu:Ph-9ekMyFDyZ9LrXeHD55ZJjD9ixehJT@woodpecker.rmq.cloudamqp.com/vlacgcyu");

using IConnection connection =  await factory.CreateConnectionAsync();
using IChannel channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(exchange:"fanout-exchange-example",type:ExchangeType.Fanout);

for (int i = 0; i < 100; i++)
{
    await Task.Delay(200);
    byte[] byteMessage = Encoding.UTF8.GetBytes($"Merhaba {i}");
    await channel.BasicPublishAsync(
        exchange: "fanout-exchange-example",
        routingKey:string.Empty,
        body: byteMessage
        );
}
Console.Read();


// Fanout Exchange ile belirtilen exhange'deki büğtün queue'lara mesaj gönderir. Direct Exchange'deki gibi bir routing key'e ihtiyaç yoktur çünkü exchangedeki belir bir queue'ya değilde bütün queue'lara mesaj gönderecektir.