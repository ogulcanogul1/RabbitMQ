using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

// Bağlantı Oluşturma
ConnectionFactory factory = new();
factory.Uri = new("amqps://vlacgcyu:Ph-9ekMyFDyZ9LrXeHD55ZJjD9ixehJT@woodpecker.rmq.cloudamqp.com/vlacgcyu");

// Bağlantı aktifleştirme
using IConnection connection = await factory.CreateConnectionAsync();
using IChannel channel = await connection.CreateChannelAsync();


// Queue oluşturma

await channel.QueueDeclareAsync(queue:"example-queue",exclusive:false,durable:true);// publisher'da yapılan konfigürasyonlar burada da yapılmalıdır.


// Queque'dan mesaj okuma
AsyncEventingBasicConsumer consumer = new(channel);

var consumerTag =  await channel.BasicConsumeAsync(queue: "example-queue",autoAck:false, consumer); // buradaki autoAck message Acknowledgement ile alakalıdır burada verdiğimiz false değeri ile consumer için mesajın onaylama süreci oluşturulur.(mesaj direk silinmez.) Böylece RabbitMQ'da varsayılan olarak mesajların kuyruktan silinme davranışı değiştirilecek ve consumer'dan onay beklenecektir.

//await channel.BasicQosAsync(prefetchSize:0,prefetchCount:1,global:false); // konfigürasyon
//prefetchSize: Bir consumer tarafından alınabilecek en büyük mesaj boyutunu byte cinsinden belirler. 0 sınırsız demektir.

//prefetchCount : Bir consumer tarafından alınabilecek en fazla mesaj sayısını belirler. 1 tek bir mesaj alınacağı anlamına gelir.

//global : Bu konfigürasyonun tüm consumer'lar için mi yoksa sadece çağrı yapılan consumer için mi geçerli olacağını belirler.

 consumer.ReceivedAsync += async (sender, e) =>
{
    // kuyruğa gelen mesajların işlendiği yerdir.
    Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
    await Task.CompletedTask;

    await channel.BasicAckAsync(e.DeliveryTag, multiple: false); // Consumer, mesajı başarıyla işlendiğine dair uyarıyı 'channel.BasicAckAsync' metodu ile gerçekleştirelecek. (Message Acknowledgement)

    //multiple parametresi : birden fazla mesaja dair onay bildirisi gönderir. Eğer true değeri  verilirse , DeliveryTag değerine sahip olan bu mesaja birlikte bundan önceki mesajlarında işlendiğini onaylar. Aksi takdirde false verilirse sadece bu mesaj için onay bildirisinde  bulunacaktır.

    //await channel.BasicNackAsync(deliveryTag: e.DeliveryTag , multiple : false , requeue : true);

    // Bazen , consumer'lar da istemsiz durumların dışında kendi kontrollerimiz neticesinde mesahları işlememek isteyebilir veyahut ilgili mesajın işlenmesini başarıyla sonuçlandırmayacağımızı anlayabiliriz.
    // Böyle durumlarda 'channel.BasicNackAsync' metodunu kullanarak RabbitMQ'ya bilgi verebilir ve mesajı tekrardan işletebiliriz.
    // Tabi burada requeue parametresi oldukça önem arz etmektedir. Bu parametre , bu consımer tarafından işlenemeyeceği ifade edilen bu mesajın tekrardan kuyruğa eklenip eklenmemesinin kararını vermektedir.
    // True değeri verdiği taktirde mesaj kuyruğa tekrardan işlenmek üzere eklenecek, false değerinde ise kuyruğa eklenmeyecek silinecektir. Sadece bu mesajın işlenmöeyeceğine dair RabbitMQ'ya bilgi verilmiş olunacaktır.

    //await channel.BasicCancelAsync(consumerTag); // BasicCancel metodu ile verilen consumerTag değerine karşılık gelen queue'daki tüm mesajlar reddedilerek , işlenmez! BasisConsume metodundan dönen değer verilir.

    //await channel.BasicRejectAsync(deliveryTag:3,requeue:false); //RabbitMQ'da kuyrukta bulunan mesajların belirli olanların consumer tarafından işlenmesini istemediğimiz durumlarda BasicReject metodunu kullanabiliriz. 
};

Console.Read();



