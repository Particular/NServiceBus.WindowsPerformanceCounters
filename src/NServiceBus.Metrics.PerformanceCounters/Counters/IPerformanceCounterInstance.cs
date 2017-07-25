using System;

interface IPerformanceCounterInstance : IDisposable
{
    void Increment();
    void IncrementBy(long value);
    long RawValue { get; set; }
}