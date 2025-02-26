

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory factory = new();
factory.Uri = new("amqps://vlacgcyu:Ph-9ekMyFDyZ9LrXeHD55ZJjD9ixehJT@woodpecker.rmq.cloudamqp.com/vlacgcyu");

using IConnection connection = await factory.CreateConnectionAsync();
using IChannel channel = await connection.CreateChannelAsync();

// 1.Adım
await channel.ExchangeDeclareAsync(exchange: "direct-exchange-example",type:ExchangeType.Direct);

// 2. Adım
string queueName = channel.QueueDeclareAsync().Result.QueueName;

// 3. Adım
await channel.QueueBindAsync(queue:queueName,exchange: "direct-exchange-example", routingKey: "direct-queue-example");

AsyncEventingBasicConsumer consumer = new(channel);

await channel.BasicConsumeAsync(queue:queueName,autoAck:true,consumer:consumer);

consumer.ReceivedAsync += async (sender, e) =>
{
    Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
};

Console.Read();


// 1. Adım : Publisher'da ki exchange ile birebir aynı isim ve type'a sahip bir exchange tanımlanmalıdır!
// 2. Adım : Publisher tarafından routing key'de bulunan değerdeki kuyruğa gönderilen mesajları kendi oluşturduğumuzu kuyruğa yönlendirerek tüketmemeiz gerekmektedir. Bunun için öncelikle bir kuyruk oluşturulmalıdır!
// 3. Adım : Publisherdan gelen değerleri consumer'da oluşturduğumuz queque'ya yönlendiriyoruz.