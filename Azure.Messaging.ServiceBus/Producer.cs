using Azure.Messaging.ServiceBus;

namespace UserService.API.Azure.Messaging.ServiceBus
{
    public class Producer
    {
        public Producer() { }

        async Task produceMessage()
        {

            var client = new ServiceBusClient("<connection_string>");
            var sender = client.CreateSender("<queue_name>");

            var message = new ServiceBusMessage("Hello, Service Bus!");

            await sender.SendMessageAsync(message);
        }
    }
}

