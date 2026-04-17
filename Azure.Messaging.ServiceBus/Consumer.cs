using Azure.Messaging.ServiceBus;
using System.Threading.Tasks;

namespace UserService.API.Azure.Messaging.ServiceBus
{
    public class Consumer
    {

        public Consumer() { }

        async Task consumeMessage()
        {
            var client = new ServiceBusClient("<connection_string>");
            //for connection string need to retrieve from Azure portal and same goes for queue name
            var processor = client.CreateProcessor("<queue_name>");

            processor.ProcessMessageAsync += async args =>
            {
               string body = args.Message.Body.ToString();
                Console.WriteLine($"Received message: {body}");
                await args.CompleteMessageAsync(args.Message);
            };
            processor.ProcessErrorAsync += async args =>
            {
                Console.WriteLine($"Error processing message: {args.Exception}");
                await Task.CompletedTask;
            };
//            return Task.CompletedTask;

           await processor.StartProcessingAsync();
        }
    }
}
