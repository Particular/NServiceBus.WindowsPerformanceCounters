using System;
using System.Threading.Tasks;
using NServiceBus;

public class MyHandler : IHandleMessages<MyCommand>
{
    public static Random random = new Random();

    public Task Handle(MyCommand message, IMessageHandlerContext context)
    {
        var waitTime = random.Next(250);

        Console.WriteLine($"Received message at {DateTime.Now}, delaying for {waitTime}ms");

        return Task.Delay(waitTime);
    }
}
