using System;
using System.Threading.Tasks;
using NServiceBus;

class Program
{
    static async Task Main()
    {
        var configuration = new EndpointConfiguration("MyEndpoint");
        configuration.SendFailedMessagesTo("error");
        configuration.UsePersistence<InMemoryPersistence>();
        configuration.UseTransport<LearningTransport>();
        configuration.EnableInstallers();
        var performanceCounters = configuration.EnableWindowsPerformanceCounters();
        performanceCounters.EnableSLAPerformanceCounters(TimeSpan.FromSeconds(1));
        var endpoint = await Endpoint.Start(configuration).ConfigureAwait(false);

        ConsoleKeyInfo readKey;
        do
        {
            await endpoint.SendLocal(new MyMessage()).ConfigureAwait(false);

            readKey = Console.ReadKey();
        } while (readKey.Key != ConsoleKey.Escape);

        await endpoint.Stop().ConfigureAwait(false);
    }
}