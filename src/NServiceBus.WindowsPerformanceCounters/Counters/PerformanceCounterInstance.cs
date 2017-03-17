using System.Diagnostics;

class PerformanceCounterInstance : IPerformanceCounterInstance
{
    public PerformanceCounterInstance(PerformanceCounter counter)
    {
        this.counter = counter;
    }

    public void Increment()
    {
        counter.Increment();
    }

    public void Dispose()
    {
        counter?.Dispose();
    }

    PerformanceCounter counter;

    public long RawValue
    {
        get { return counter.RawValue; }
        set { counter.RawValue = value; }
    }
}
