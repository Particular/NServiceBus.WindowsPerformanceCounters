using System;
using System.Collections.Generic;
using System.Threading;
using NServiceBus;
using NServiceBus.Transport;
using NServiceBus.WindowsPerformanceCounters;
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
            sentInstant: new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc),
            processingStarted: new DateTime(2000, 1, 1, 1, 1, 2, DateTimeKind.Utc),
            processingEnded: new DateTime(2000, 1, 1, 1, 1, 3, DateTimeKind.Utc));
        Assert.AreEqual(2, mockPerformanceCounter.RawValue);
    }

    [Test]
    public void Should_extract_timesent_from_headers()
    {
        var mockPerformanceCounter = new MockIPerformanceCounter();
        var counter = new CriticalTimeCounter(mockPerformanceCounter);
        var headers = new Dictionary<string, string>
        {
            {
                Headers.TimeSent, DateTimeExtensions.ToWireFormattedString(new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc))
            }
        };
        var pipelineCompleted = BuildPipelineCompleted(
            headers: headers,
            startedAt: new DateTime(2000, 1, 1, 1, 1, 2, DateTimeKind.Utc),
            completedAt: new DateTime(2000, 1, 1, 1, 1, 3, DateTimeKind.Utc));
        counter.Update(pipelineCompleted);
        Assert.AreEqual(2, mockPerformanceCounter.RawValue);
    }

    [Test]
    public void Timer_should_reset_rawvalue()
    {
        var mockPerformanceCounter = new MockIPerformanceCounter();
        using (var counter = new CriticalTimeCounter(mockPerformanceCounter))
        {
            counter.Update(
                sentInstant: new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc),
                processingStarted: new DateTime(2000, 1, 1, 1, 1, 2, DateTimeKind.Utc),
                processingEnded: new DateTime(2000, 1, 1, 1, 1, 3, DateTimeKind.Utc));
            counter.Start();
            Thread.Sleep(100);
        }
        Assert.AreEqual(0, mockPerformanceCounter.RawValue);
    }

    static ReceivePipelineCompleted BuildPipelineCompleted(Dictionary<string, string> headers, DateTime startedAt, DateTime completedAt)
    {
        var message = new IncomingMessage("id", headers, new byte[]
        {
        });
        return new ReceivePipelineCompleted(message, startedAt, completedAt);
    }
}