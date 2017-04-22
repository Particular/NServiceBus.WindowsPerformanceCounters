public class MockIPerformanceCounter : IPerformanceCounterInstance
{
    public bool Disposed { get; private set; }

    public void Dispose()
    {
        Disposed = true;
    }

    public void Increment()
    {
        RawValue++;
    }

    public long RawValue { get; set; }
}