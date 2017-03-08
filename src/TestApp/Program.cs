using System;
using System.Threading.Tasks;

namespace TestApp
{
    using NServiceBus;
    using NServiceBus.WindowsPerformanceCounters;

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
            
            //
            // *** The current way
            //
            endpointConfig.EnablePerformanceCounters();
            //endpointConfig.EnableSLAPerformanceCounter(TimeSpan.FromSeconds(100));

            //
            // The extensible way
            //
            endpointConfig.EnableMetrics().
                EnableSLAPerformanceCounters().
                EnableStatistics().
                Whatever();

            //
            // The recoverability way
            //
            endpointConfig.WindowsPerformanceCounters()
                .EnableSLACounters(s =>
                {
                    s.EndpointSLATimeout(TimeSpan.FromMinutes(10));
                });

            var endpoint = await Endpoint.Start(endpointConfig);

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
        public Task Handle(MyCommand message, IMessageHandlerContext context)
        {
            Console.WriteLine($"Received message at {DateTime.Now}");
            return Task.FromResult(0);
        }
    }
}

namespace NServiceBus
{
    public static class MetricsExtensions
    {
        public static Metrics Whatever(this Metrics metrics)
        {
            return metrics;
        }
    }
}