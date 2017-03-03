using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            
            endpointConfig.EnablePerformanceCounters();

            var endpoint = await Endpoint.Start(endpointConfig);

            while (true)
            {
                var cmd = new MyCommand();
                await endpoint.SendLocal(cmd);
                await Task.Delay(1000);
            }
        }
    }

    public class MyCommand
    {
        
    }
    public class MyHandler : IHandleMessages<MyCommand>
    {
        public Task Handle(MyCommand message, IMessageHandlerContext context)
        {
            return Task.FromResult(0);
        }
    }
}
