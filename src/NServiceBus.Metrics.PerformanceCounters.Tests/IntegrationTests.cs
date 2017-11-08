using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using NUnit.Framework;

[TestFixture]
public class IntegrationTests
{
    const string EndpointName = "PerfCountersIntegrationTests";
    static ManualResetEvent ManualResetEvent = new ManualResetEvent(false);

    [Test]
    public async Task Ensure_counters_are_written()
    {
        string message = null;

        var endpointConfiguration = EndpointConfigBuilder.BuildEndpoint(EndpointName);
        endpointConfiguration.DefineCriticalErrorAction(
            context =>
            {
                message = context.Error;
                ManualResetEvent.Set();
                return Task.FromResult(0);
            });

        var performanceCounters = endpointConfiguration.EnableWindowsPerformanceCounters();
        performanceCounters.EnableSLAPerformanceCounters(TimeSpan.FromSeconds(10));

        var endpoint = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);

        await endpoint.SendLocal(new MyMessage())
            .ConfigureAwait(false);

        ManualResetEvent.WaitOne();
        await Task.Delay(1500)
            .ConfigureAwait(false);
        await endpoint.Stop()
            .ConfigureAwait(false);

        var criticalTimePerfCounter = GetCounter(PerformanceCountersFeature.CriticalTimeCounterName);
        var processingTimePerfCounter = GetCounter(PerformanceCountersFeature.ProcessingTimeCounterName);
        var slaPerCounter = GetCounter(SLAMonitoringFeature.CounterName);
        var messagesFailuresPerSecondCounter = GetCounter(PerformanceCountersFeature.MessagesFailuresPerSecondCounterName);
        var messagesProcessedPerSecondCounter = GetCounter(PerformanceCountersFeature.MessagesProcessedPerSecondCounterName);
        var messagesPulledPerSecondCounter = GetCounter(PerformanceCountersFeature.MessagesPulledPerSecondCounterName);
        //Assert.AreNotEqual(0, criticalTimePerfCounter.RawValue);
        //Assert.AreNotEqual(0, processingTimePerfCounter.RawValue);
        Assert.AreNotEqual(0, slaPerCounter.RawValue);
        Assert.AreEqual(0, messagesFailuresPerSecondCounter.RawValue);
        Assert.AreNotEqual(0, messagesProcessedPerSecondCounter.RawValue);
        Assert.AreNotEqual(0, messagesPulledPerSecondCounter.RawValue);

        Assert.IsNull(message);
    }

    static PerformanceCounter GetCounter(string counterName) => new PerformanceCounter("NServiceBus", counterName, EndpointName, true);

    public class MyHandler : IHandleMessages<MyMessage>
    {
        public Task Handle(MyMessage message, IMessageHandlerContext context)
        {
            ManualResetEvent.Set();
            return Task.Delay(TimeSpan.FromMilliseconds(1000));
        }
    }
    public class MyMessage : ICommand
    {

    }

}