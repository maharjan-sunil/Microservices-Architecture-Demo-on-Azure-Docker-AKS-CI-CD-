using DockerDemo.Docker.Model;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace UserService.API.RabbitMQ
{
    public  class StoreConsumer : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare("store-queue", true, false, false);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var store = JsonSerializer.Deserialize<Store>(message);

                Console.WriteLine($"Processing store: {store.Name}");

                // Simulate work
                Thread.Sleep(10000);

                Console.WriteLine($"Store Approved: {store.Name}");
            };

            channel.BasicConsume("store-queue", true, consumer);

            return Task.CompletedTask;
        }
    }
}
