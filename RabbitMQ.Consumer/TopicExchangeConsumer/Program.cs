using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory factory = new();

factory.Uri = new("amqps://vlacgcyu:Ph-9ekMyFDyZ9LrXeHD55ZJjD9ixehJT@woodpecker.rmq.cloudamqp.com/vlacgcyu");

using IConnection connection = await factory.CreateConnectionAsync();
using IChannel channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(exchange: "topic-exchange-example", type: ExchangeType.Topic);

Console.Write("Dinlenecek topic formatını belritiniz : ");
string? topicType = Console.ReadLine();

string queueName = channel.QueueDeclareAsync().Result.QueueName;

await channel.QueueBindAsync(queue: queueName, routingKey:topicType , exchange: "topic-exchange-example");

AsyncEventingBasicConsumer consumer = new(channel);

consumer.ReceivedAsync += async (sender, e) => 
{
    Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
    await Task.CompletedTask;
};

Console.Read();


/*
 # ne gelirse gelsin, sıfır veya daha fazla kelimeyi temsil eder.
* ise sadece 1 kelimeyi temsil eder.
 
log.*.error =>	log.api.error, log.db.error

log.#       =>	Tüm log ile başlayan mesajlar

*.api.*     =>	log.api.info, log.api.error
 */