using System.Threading.Tasks;
using NServiceBus;

public class MyHandler : IHandleMessages<MyMessage>
{
    public Task Handle(MyMessage message, IMessageHandlerContext context)
    {
        return Task.Delay(2000);
    }
}