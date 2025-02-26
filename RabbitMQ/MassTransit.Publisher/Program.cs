
using MassTransit;
using MassTransit.Shared.Messages;

string rabbitMQUri = "amqps://vlacgcyu:Ph-9ekMyFDyZ9LrXeHD55ZJjD9ixehJT@woodpecker.rmq.cloudamqp.com/vlacgcyu";

string queueName = "example-queue";

IBusControl bus =  Bus.Factory.CreateUsingRabbitMq(factory =>
{
    factory.Host(rabbitMQUri);
});


ISendEndpoint sendEndpoint =  await bus.GetSendEndpoint(new($"{rabbitMQUri}/{queueName}"));

Console.Write("Gönderilecek Mesaj : ");
string message = Console.ReadLine();

await sendEndpoint.Send<IMessage>(new ExampleMessage
{
    Text = message
});


Console.Read();

