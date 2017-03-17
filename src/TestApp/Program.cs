using System;
using System.Threading.Tasks;

namespace TestApp
{
    using NServiceBus;
    
    class Program
    {
        static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public async Task MainAsync()
        {
            var endpointConfig = new EndpointConfiguration("TestEndpoint");
            endpointConfig.UsePersistence<InMemoryPersistence>();
            endpointConfig.SendFailedMessagesTo("error");
            endpointConfig.EnableInstallers();

            endpointConfig.EnableWindowsPerformanceCounters()
                .EnableSLAPerformanceCounters(TimeSpan.FromSeconds(10));

            var endpoint = await Endpoint.Start(endpointConfig);

            Console.WriteLine("Press a key to start sending messages...");
            Console.ReadKey();

            var rnd = new Random();
            while (true)
            {
                var cmd = new MyCommand();
                await endpoint.SendLocal(cmd);
                await Task.Delay(rnd.Next(250));
            }
        }
    }

    public class MyCommand : ICommand
    {
        
    }
    
    public class MyHandler : IHandleMessages<MyCommand>
    {
        public static Random random = new Random();

        public async Task Handle(MyCommand message, IMessageHandlerContext context)
        {
            var waitTime = random.Next(250);

            Console.WriteLine($"Received message at {DateTime.Now}, delaying for {waitTime}ms");

            await Task.Delay(waitTime);
        }
    }
}