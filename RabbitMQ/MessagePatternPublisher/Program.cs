
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Text;

ConnectionFactory factory = new()
{
    Uri = new("amqps://vlacgcyu:Ph-9ekMyFDyZ9LrXeHD55ZJjD9ixehJT@woodpecker.rmq.cloudamqp.com/vlacgcyu")
};


using IConnection connection = await factory.CreateConnectionAsync();
using IChannel channel = await connection.CreateChannelAsync();

#region P2P(Point to Point)
//string queueName = "example-p2p-queue";

//await channel.QueueDeclareAsync(
//    queue: queueName,
//    durable: false,
//    exclusive: false,
//    autoDelete: false);

//await Task.Delay(1000);

//byte[] body = Encoding.UTF8.GetBytes("Merhaba");

//await channel.BasicPublishAsync(exchange: "", routingKey: queueName, body: body);
#endregion

#region Publish Subscribe (Pub/Sub) Tasarımı
//string exchange = "fanout-exchange-example";
//await channel.ExchangeDeclareAsync(exchange:exchange,ExchangeType.Fanout,autoDelete:false);

//byte[] byteMessage = Encoding.UTF8.GetBytes("Merhaba PubSub");

//await channel.BasicPublishAsync(exchange:exchange,routingKey:string.Empty,body: byteMessage);

// Bu tasarımda publisher mesajı bir exchange'e gönderir ve böylece mesaj bu exchange'e bind edilmiş olan tüm kuyruklara yönlendirilir. Bu tasarım , bir mesajın birçok tüketici tarafından işlenmesi gerektiği durumlarda kullanışlıdır.
#endregion

#region Work Queue (İş Kuyruğu) Tasarımı 
//string queueName = "workqueue-queue-example";
//await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false,autoDelete:true);


//var basicProperties = new BasicProperties()
//{
//    Persistent = true,
//    // durable (true) ve bu ayar redis sunucusu kesildiğinde verilerin kaydedilmesini sağlar fakat garanti vermez.
//};

//for (int i = 0; i < 100; i++)
//{
//    byte[] byteMessage = Encoding.UTF8.GetBytes($"Merhaba {i}");

//    await channel.BasicPublishAsync(
//         exchange: string.Empty,
//         routingKey: queueName,
//         body: byteMessage,
//         mandatory: false,
//         basicProperties:basicProperties);

//}

// Bu tasarıdma , publisher tarafından yayınlanmış bir mesajın birden fazla consumer arasından yalnızca biri tarafından tüketilmesi amaçlanmaktadır. Böylece mesajların işlenmesi sürecinde tüm consumer'lar aynı iş yüküne ve eşit görev dağılımına sahip olacaklardır. Genellikle Direct Exhange kullanılır.
#endregion

#region Request/Response Pattern

string requestQueueName = "request-queue-example";
await channel.QueueDeclareAsync(queue:requestQueueName,durable:false,exclusive:false,autoDelete:true);

string replyQueueName = channel.QueueDeclareAsync().Result.QueueName;

string correlationId = Guid.NewGuid().ToString();

#region request queue'ya mesajı yollama

BasicProperties properties = new()
{
    CorrelationId = correlationId,
    ReplyTo = replyQueueName
};

for(int i = 0; i < 10; i++)
{
    byte[] byteMessage = Encoding.UTF8.GetBytes("Merhaba" + i);
    await channel.BasicPublishAsync(
        exchange: string.Empty,
        routingKey: requestQueueName,
        body:byteMessage,
        mandatory:false,
        basicProperties:properties);
}

#endregion

#region Response Kuyruğu Dinleme
AsyncEventingBasicConsumer consumer = new(channel);

await channel.BasicConsumeAsync(queue:replyQueueName,autoAck:true,consumer:consumer);

consumer.ReceivedAsync += async (sender, e) =>
{
    if (e.BasicProperties.CorrelationId == correlationId)
    {
        Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
    }
};
#endregion

// Bu tasarımda,  publisher bir request yapar gibi kuyruğa mesaj gönderir ve bu mesajı tüketen consumer'dan sonuca dair başka kuyruktan bir yanıt/response bekler. Bu tarz senaryolar için oldukça uygun bir tasarımdır. Yani publisher ve consumer , hem publisher hem de consumer işlevi görür.
#endregion





Console.Read();
