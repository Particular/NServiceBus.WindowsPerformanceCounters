using System;
using System.Threading.Tasks;
using NServiceBus;

class Program
{
    static void Main()
    {
        MainAsync().GetAwaiter().GetResult();
    }

    static async Task MainAsync()
    {
        var endpointConfig = new EndpointConfiguration("TestEndpoint");
        endpointConfig.UsePersistence<InMemoryPersistence>();
        endpointConfig.SendFailedMessagesTo("error");
        endpointConfig.EnableInstallers();

        var performanceCounters = endpointConfig.EnableWindowsPerformanceCounters();
        performanceCounters.EnableSLAPerformanceCounters(TimeSpan.FromSeconds(10));

        var endpoint = await Endpoint.Start(endpointConfig).ConfigureAwait(false);

        Console.WriteLine("Press a key to start sending messages...");
        Console.ReadKey();

        var rnd = new Random();
        while (true)
        {
            var cmd = new MyCommand();
            await endpoint.SendLocal(cmd).ConfigureAwait(false);
            await Task.Delay(rnd.Next(250)).ConfigureAwait(false);
        }
    }
}