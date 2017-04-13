using System;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Runtime.CompilerServices;

// Currently copied here until we find a way to dynamically create and reference it.
[CompilerGenerated]
public static class CounterCreator 
{
    public static void Create() 
    {
        var counterCreationCollection = new CounterCreationDataCollection(Counters);
        try
        {
            var install = false;
            var categoryName = "NServiceBus";
            if (PerformanceCounterCategory.Exists(categoryName))
            {
                var category = PerformanceCounterCategory.GetCategories().Single(x => x.CategoryName == categoryName);
                var existingCounters = category.GetCounters();

                if (existingCounters.Length != counterCreationCollection.Count)
                {
                    install = true;
                }
                else
                {
                    foreach (var counter in Counters)
                    {
                        var foundCounter = existingCounters.FirstOrDefault(c => c.CounterName == counter.CounterName);
                        var found = foundCounter?.CounterName == counter.CounterName | foundCounter?.CounterType == counter.CounterType | foundCounter?.CounterHelp == counter.CounterHelp;
                        if (!found)
                        {
                            install = true;
                        }
                    }
                }
            }

            if (install)
            {
                PerformanceCounterCategory.Create(
                    categoryName: categoryName,
                    categoryHelp: "NServiceBus statistics",
                    categoryType: PerformanceCounterCategoryType.MultiInstance,
                    counterData: counterCreationCollection);
                PerformanceCounter.CloseSharedResources();
            }
        } catch(Exception ex) when(ex is SecurityException || ex is UnauthorizedAccessException)
        {
            throw new Exception("Execution requires elevated permissions", ex);
        }
    }

    static CounterCreationData[] Counters = new CounterCreationData[]
    {
        new CounterCreationData("SLA violation countdown", "Seconds until the SLA for this endpoint is breached.", PerformanceCounterType.NumberOfItems32),
        new CounterCreationData("Critical Time", "Age of the oldest message in the queue.", PerformanceCounterType.NumberOfItems32),
        new CounterCreationData("Processing Time", "The time it took to successfully process a message.", PerformanceCounterType.NumberOfItems32),
        new CounterCreationData("# of msgs pulled from the input queue /sec", "The current number of messages pulled from the input queue by the transport per second.", PerformanceCounterType.RateOfCountsPerSecond32),
        new CounterCreationData("# of msgs failures / sec", "The current number of failed processed messages by the transport per second.", PerformanceCounterType.RateOfCountsPerSecond32),
        new CounterCreationData("# of msgs successfully processed / sec", "The current number of messages processed successfully by the transport per second.", PerformanceCounterType.RateOfCountsPerSecond32),

    };
}