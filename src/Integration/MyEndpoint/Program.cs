using System;
using System.Threading.Tasks;
using NServiceBus;

namespace MyEndpoint
{
    class Program
    {
        static void Main(string[] args)
        {
            AsyncMain().GetAwaiter().GetResult();
        }

        static async Task AsyncMain()
        {
            var configuration = new EndpointConfiguration("MyEndpoint");
            configuration.SendFailedMessagesTo("error");
            configuration.UsePersistence<InMemoryPersistence>();
            configuration.UseTransport<MsmqTransport>();
            configuration.EnableInstallers();
            var performanceCounters = configuration.EnableWindowsPerformanceCounters();
            performanceCounters.EnableSLAPerformanceCounters(TimeSpan.FromSeconds(1));
            var endpoint = await Endpoint.Start(configuration);

            ConsoleKeyInfo readKey;
            do
            {
                await endpoint.SendLocal(new MyMessage());

                readKey = Console.ReadKey();
            } while (readKey.Key != ConsoleKey.Escape);

            await endpoint.Stop();
        }
    }

    public class MyMessage : ICommand
    {
    }

    public class MyHandler : IHandleMessages<MyMessage> {
        public Task Handle(MyMessage message, IMessageHandlerContext context)
        {
            return Task.Delay(2000);
        }
    }
}
