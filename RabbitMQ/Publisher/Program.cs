
using RabbitMQ.Client;
using System.Text;

// Bağlamntı Oluşturma
ConnectionFactory factory = new();
factory.Uri = new("amqps://vlacgcyu:Ph-9ekMyFDyZ9LrXeHD55ZJjD9ixehJT@woodpecker.rmq.cloudamqp.com/vlacgcyu");


// Bağlantıyı Aktifleştirme ve Kanal Açma
using IConnection connection = await factory.CreateConnectionAsync();
using IChannel channel = await connection.CreateChannelAsync();

// Queue oluşturma
await channel.QueueDeclareAsync(queue: "example-queue", exclusive:false, durable:true); // Durable mesajları kalıcı yapmak için sunucu giderse vs

// Queue'ya mesaj gönderme
// RabbitMQ kuruğa atacağı mesajşarı byte türünden kabul etmektedir. Haliyle mesajları izim byte dönüşmemiz gerecektir.
byte[] message = Encoding.UTF8.GetBytes("Merhaba123");
var properties = new BasicProperties // mesajın kalıcılığı için yapılır sunucu giderse vs
{
    Persistent = true,
};

//await channel.BasicPublishAsync(exchange:"",routingKey: "example-queue", body:message);
//  default olarak direct-exchange kullanılır.

await channel.BasicPublishAsync(exchange: "",
    routingKey: "example-queue",
    mandatory: false,
    basicProperties: properties, // mesajları kalıcı yapmak için
    body: message);

Console.Read();


//mandatory Parametresinin Anlamı:
//true(Zorunlu):

//Mesaj, belirtilen routingKey ile eşleşen bir kuyruk bulamazsa, mesaj geri döner ve bir hata bildirimi alırsınız.
//Eğer bir kuyruk yoksa, BasicReturn olayı tetiklenir.
//false (Varsayılan):

//Mesaj bir kuyrukla eşleşmezse sessizce kaybolur. Hiçbir hata veya bildirim almazsınız.