using System;
using NUnit.Framework;

[TestFixture]
public class EstimatedTimeToSLABreachCounterTests
{
    [Test]
    public void Single_Datapoint_should_result_in_MaxValue()
    {
        var mockPerformanceCounter = new MockIPerformanceCounter();
        var counter = new EstimatedTimeToSLABreachCounter(TimeSpan.FromSeconds(2), mockPerformanceCounter);
        counter.Update(
            sent: new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc),
            processingStarted: new DateTime(2000, 1, 1, 1, 1, 2, DateTimeKind.Utc),
            processingEnded: new DateTime(2000, 1, 1, 1, 1, 3, DateTimeKind.Utc));
        Assert.AreEqual(int.MaxValue, mockPerformanceCounter.RawValue);
    }

    [Test]
    public void Exceed_SLA_should_result_in_Zero()
    {
        var mockPerformanceCounter = new MockIPerformanceCounter();
        var counter = new EstimatedTimeToSLABreachCounter(TimeSpan.FromSeconds(2), mockPerformanceCounter);
        counter.Update(
            sent: new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc),
            processingStarted: new DateTime(2000, 1, 1, 1, 1, 2, DateTimeKind.Utc),
            processingEnded: new DateTime(2000, 1, 1, 1, 1, 3, DateTimeKind.Utc));
        counter.Update(
            sent: new DateTime(2000, 1, 1, 1, 1, 4, DateTimeKind.Utc),
            processingStarted: new DateTime(2000, 1, 1, 1, 1, 5, DateTimeKind.Utc),
            processingEnded: new DateTime(2000, 1, 1, 1, 1, 10, DateTimeKind.Utc));
        Assert.AreEqual(0, mockPerformanceCounter.RawValue);
    }

    [Test]
    public void Within_SLA_should_result_in_difference()
    {
        var mockPerformanceCounter = new MockIPerformanceCounter();
        var counter = new EstimatedTimeToSLABreachCounter(TimeSpan.FromSeconds(20), mockPerformanceCounter);
        counter.Update(
            sent: new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc),
            processingStarted: new DateTime(2000, 1, 1, 1, 1, 2, DateTimeKind.Utc),
            processingEnded: new DateTime(2000, 1, 1, 1, 1, 3, DateTimeKind.Utc));
        counter.Update(
            sent: new DateTime(2000, 1, 1, 1, 1, 4, DateTimeKind.Utc),
            processingStarted: new DateTime(2000, 1, 1, 1, 1, 5, DateTimeKind.Utc),
            processingEnded: new DateTime(2000, 1, 1, 1, 1, 10, DateTimeKind.Utc));
        Assert.AreEqual(24, mockPerformanceCounter.RawValue);
    }

}