using System;
using System.Threading;
using NUnit.Framework;

[TestFixture]
public class CriticalTimeCounterTests
{
    [Test]
    public void Update_should_be_reflected_in_rawvalue()
    {
        var mockPerformanceCounter = new MockIPerformanceCounter();
        var counter = new CriticalTimeCounter(mockPerformanceCounter);
        counter.Update(
            sent: new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc),
            processingStarted: new DateTime(2000, 1, 1, 1, 1, 2, DateTimeKind.Utc),
            processingEnded: new DateTime(2000, 1, 1, 1, 1, 3, DateTimeKind.Utc));
        Assert.AreEqual(2, mockPerformanceCounter.RawValue);
    }

    [Test]
    public void Timer_should_reset_rawvalue()
    {
        var mockPerformanceCounter = new MockIPerformanceCounter();
        using (var counter = new CriticalTimeCounter(mockPerformanceCounter))
        {
            counter.Update(
                sent: new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc),
                processingStarted: new DateTime(2000, 1, 1, 1, 1, 2, DateTimeKind.Utc),
                processingEnded: new DateTime(2000, 1, 1, 1, 1, 3, DateTimeKind.Utc));
            counter.Start();
            Thread.Sleep(100);
        }
        Assert.AreEqual(0, mockPerformanceCounter.RawValue);
    }
    
}