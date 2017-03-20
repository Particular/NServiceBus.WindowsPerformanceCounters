﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using NUnit.Framework;

[TestFixture]
public class IntegrationTests
{
    static ManualResetEvent ManualResetEvent = new ManualResetEvent(false);

    [Test]
    public async Task Ensure_counters_are_written()
    {
        string message = null;

        var endpointName = "PerfCountersIntegrationTests";
        var endpointConfiguration = EndpointConfigBuilder.BuildEndpoint(endpointName);
        var typesToScan = TypeScanner.NestedTypes<IntegrationTests>().ToList();
        endpointConfiguration.SetTypesToScan(typesToScan);
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
        await Task.Delay(100)
            .ConfigureAwait(false);
        await endpoint.Stop()
            .ConfigureAwait(false);

        var criticalTimePerfCounter = new PerformanceCounter("NServiceBus", CriticalTimeFeature.CounterName, endpointName, true);
        var slaPerCounter = new PerformanceCounter("NServiceBus", SLAMonitoringFeature.CounterName, endpointName, true);
        var messagesFailuresPerSecondCounter = new PerformanceCounter("NServiceBus", ReceivePerformanceDiagnosticsBehavior.MessagesFailuresPerSecondCounterName, endpointName, true);
        var messagesProcessedPerSecondCounter = new PerformanceCounter("NServiceBus", ReceivePerformanceDiagnosticsBehavior.MessagesProcessedPerSecondCounterName, endpointName, true);
        var messagesPulledPerSecondCounter = new PerformanceCounter("NServiceBus", ReceivePerformanceDiagnosticsBehavior.MessagesPulledPerSecondCounterName, endpointName, true);
        Assert.AreNotEqual(0, criticalTimePerfCounter.RawValue);
        Assert.AreNotEqual(0, slaPerCounter.RawValue);
        Assert.AreEqual(0, messagesFailuresPerSecondCounter.RawValue);
        Assert.AreNotEqual(0, messagesProcessedPerSecondCounter.RawValue);
        Assert.AreNotEqual(0, messagesPulledPerSecondCounter.RawValue);

        Assert.IsNull(message);
    }

    public class MyHandler : IHandleMessages<MyMessage>
    {
        public Task Handle(MyMessage message, IMessageHandlerContext context)
        {
            ManualResetEvent.Set();
            return Task.Delay(TimeSpan.FromMilliseconds(100));
        }
    }
    public class MyMessage : ICommand
    {

    }

}