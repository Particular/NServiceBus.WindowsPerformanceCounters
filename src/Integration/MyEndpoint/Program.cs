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
            await Endpoint.Start(configuration);
        }
    }
}
