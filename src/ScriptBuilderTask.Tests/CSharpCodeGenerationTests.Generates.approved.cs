using System;
using System.Diagnostics;
using System.Security;
using System.Runtime.CompilerServices;

[CompilerGenerated]
public static class CounterCreator 
{
    public static void Create() 
    {
        var counterCreationCollection = new CounterCreationDataCollection(Counters);
        try
        {
            var categoryName = "NServiceBus";
            if (PerformanceCounterCategory.Exists(categoryName))
            {
                PerformanceCounterCategory.Delete(categoryName);
            }

            PerformanceCounterCategory.Create(
                categoryName: categoryName,
                categoryHelp: "NServiceBus statistics",
                categoryType: PerformanceCounterCategoryType.MultiInstance,
                counterData: counterCreationCollection);
            PerformanceCounter.CloseSharedResources();
        } catch(Exception ex) when(ex is SecurityException || ex is UnauthorizedAccessException)
        {
            throw new Exception("Execution requires elevated permissions", ex);
        }
    }

    static CounterCreationData[] Counters = new CounterCreationData[]
    {
        new CounterCreationData("SLA violation countdown", "SLA Violation Countdown - Seconds until the SLA for this endpoint is breached. The value is in rounded seconds. This is an instantaneous snapshot, not an average over the time interval.", PerformanceCounterType.NumberOfItems32),
        new CounterCreationData("Avg. Critical Time", "Avg. Critical Time - The time it took from sending to processing the message. The value is the average duration in seconds during the sample interval.", PerformanceCounterType.AverageTimer32),
        new CounterCreationData("Avg. Critical TimeBase", "Avg. Critical Time - The time it took from sending to processing the message. The value is the average duration in seconds during the sample interval.", PerformanceCounterType.AverageBase),
        new CounterCreationData("Critical Time", "Critical time - The time it took from sending to processing the message. The value is in rounded seconds. This is an instantaneous snapshot, not an average over the time interval.", PerformanceCounterType.NumberOfItems32),
        new CounterCreationData("Avg. Processing Time", "Avg. Processing Time - The time it took to successfully process a message. The value is the average duration in seconds during the sample interval.", PerformanceCounterType.AverageTimer32),
        new CounterCreationData("Avg. Processing TimeBase", "Avg. Processing Time - The time it took to successfully process a message. The value is the average duration in seconds during the sample interval.", PerformanceCounterType.AverageBase),
        new CounterCreationData("Processing Time", "Processing time - The time it took to successfully process a message. The value is in rounded seconds. This is an instantaneous snapshot, not an average over the time interval.", PerformanceCounterType.NumberOfItems32),
        new CounterCreationData("# of msgs failures / sec", "The current number of failed processed messages by the transport per second.", PerformanceCounterType.RateOfCountsPerSecond32),
        new CounterCreationData("# of msgs successfully processed / sec", "The current number of messages processed successfully by the transport per second.", PerformanceCounterType.RateOfCountsPerSecond32),
        new CounterCreationData("# of msgs pulled from the input queue /sec", "The current number of messages pulled from the input queue by the transport per second.", PerformanceCounterType.RateOfCountsPerSecond32),

    };
}