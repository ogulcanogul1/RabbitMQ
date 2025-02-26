
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory factory = new()
{
    Uri = new("amqps://vlacgcyu:Ph-9ekMyFDyZ9LrXeHD55ZJjD9ixehJT@woodpecker.rmq.cloudamqp.com/vlacgcyu")
};

using IConnection connection = await factory.CreateConnectionAsync();
using IChannel channel = await connection.CreateChannelAsync();

#region P2P(Point-to-Point) Tasarımı
//string queueName = "example-p2p-queue";
//await channel.QueueDeclareAsync(
//    queue: queueName,
//    durable: false,
//    exclusive: false,
//    autoDelete: false
//    );
//AsyncEventingBasicConsumer consumer = new(channel);
//await channel.BasicConsumeAsync(queueName, autoAck: true, consumer);

//consumer.ReceivedAsync += async (sender, e) =>
//{
//    Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
//};

// direk queueya mesaj iletilir ve o queue'nun consume'u/consumer'ları direk queuedan mesajı alır.
#endregion

#region Publish/Subscribe (Pub/Sub) Tasarımı
//string exchange = "fanout-exchange-example";
//Console.Write("Queue Name:");
//string? queueName = Console.ReadLine();
//await channel.ExchangeDeclareAsync(exchange:exchange,type:ExchangeType.Fanout,autoDelete:false);

//await channel.QueueDeclareAsync(queue:queueName, durable:false,exclusive:false);

//await channel.QueueBindAsync(queue:queueName,exchange:exchange,routingKey:string.Empty);

//await channel.BasicQosAsync(prefetchSize:0,prefetchCount:1,global:false);

//AsyncEventingBasicConsumer consumer = new(channel);

//await channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);

//consumer.ReceivedAsync += async (sender, e) =>
//{
//    Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
//};

#endregion


#region Work Queue (İş Kuyruğu) Tasarımı
//string queueName = "workqueue-queue-example";
//await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: true);

//AsyncEventingBasicConsumer consumer = new(channel);

//await channel.BasicConsumeAsync(queue:queueName,autoAck:true,consumer:consumer);

//await channel.BasicQosAsync(
//    prefetchSize: 0,
//    prefetchCount: 1,
//    global: false);

//consumer.ReceivedAsync += async (sender, e) =>
//{
//    Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
//};
#endregion

#region Request/Response Tasarımı
string requestQueueName = "request-queue-example";
await channel.QueueDeclareAsync(queue: requestQueueName, durable: false, exclusive: false, autoDelete: true);
AsyncEventingBasicConsumer consumer = new(channel);

await channel.BasicConsumeAsync(queue:requestQueueName,autoAck:true,consumer:consumer);

consumer.ReceivedAsync += async (sender, e) =>
{
    string message = Encoding.UTF8.GetString(e.Body.Span);
    Console.WriteLine(message);

    // --- mesaj okundu response queueya response gönderilecek
    byte[] responseMessage = Encoding.UTF8.GetBytes($"İşlem Tamalandı:{message}");

    BasicProperties properties = new() { 
     CorrelationId = e.BasicProperties.CorrelationId,
    };

    await channel.BasicPublishAsync(exchange:string.Empty,routingKey:e.BasicProperties.ReplyTo,body:responseMessage,mandatory:false,basicProperties:properties);
};

#endregion
Console.Read();
