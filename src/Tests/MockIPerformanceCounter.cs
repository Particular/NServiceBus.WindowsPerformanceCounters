using NServiceBus;

public class MockIPerformanceCounter : IPerformanceCounterInstance
{
    public void Dispose()
    {
    }

    public void Increment()
    {
        RawValue++;
    }

    public long RawValue { get; set; }
}