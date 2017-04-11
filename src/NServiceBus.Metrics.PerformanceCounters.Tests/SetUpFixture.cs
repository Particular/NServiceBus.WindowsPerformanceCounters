using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security;
using NUnit.Framework;

[SetUpFixture]
public class SetUpFixture
{
    [OneTimeSetUp]
    public void SetUp()
    {
        var counterCreationCollection = new CounterCreationDataCollection(Counters.ToArray());
        try
        {
            if (!PerformanceCounterCategory.Exists("NServiceBus"))
            {
                PerformanceCounterCategory.Create(
                    categoryName: "NServiceBus",
                    categoryHelp: "NServiceBus statistics",
                    categoryType: PerformanceCounterCategoryType.MultiInstance,
                    counterData: counterCreationCollection);
                PerformanceCounter.CloseSharedResources();
            }
        }
        catch(Exception ex) when(ex is SecurityException || ex is UnauthorizedAccessException)
        {
            throw new Exception("Run VS as admin", ex);
        }
    }

    static List<CounterCreationData> Counters = new List<CounterCreationData>
    {
        new CounterCreationData(PerformanceCountersFeature.CriticalTimeCounterName, "Age of the oldest message in the queue.", PerformanceCounterType.NumberOfItems32),
        new CounterCreationData(SLAMonitoringFeature.CounterName, "Seconds until the SLA for this endpoint is breached.", PerformanceCounterType.NumberOfItems32),
        new CounterCreationData(PerformanceCountersFeature.MessagesProcessedPerSecondCounterName, "The current number of messages processed successfully by the transport per second.", PerformanceCounterType.RateOfCountsPerSecond32),
        new CounterCreationData(PerformanceCountersFeature.MessagesPulledPerSecondCounterName, "The current number of messages pulled from the input queue by the transport per second.", PerformanceCounterType.RateOfCountsPerSecond32),
        new CounterCreationData(PerformanceCountersFeature.MessagesFailuresPerSecondCounterName, "The current number of failed processed messages by the transport per second.", PerformanceCounterType.RateOfCountsPerSecond32)
    };
}