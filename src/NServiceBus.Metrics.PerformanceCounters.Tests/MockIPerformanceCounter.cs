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

    public void IncrementBy(long value)
    {
        RawValue += value;
    }

    public long RawValue { get; set; }
}