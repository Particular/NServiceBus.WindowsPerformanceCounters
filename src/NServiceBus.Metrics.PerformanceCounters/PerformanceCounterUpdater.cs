using System.Collections.Generic;
using NServiceBus;

class PerformanceCounterUpdater
{
    public PerformanceCounterUpdater(PerformanceCountersCache cache, Dictionary<string, CounterInstanceName?> legacyInstanceNameMap, string endpointName)
    {
        this.legacyInstanceNameMap = legacyInstanceNameMap;
        this.endpointName = endpointName;
        this.cache = cache;
    }

    public void Observe(ProbeContext context)
    {
        foreach (var sp in context.Signals)
        {
            CounterInstanceName? instanceName;
            legacyInstanceNameMap.TryGetValue(sp.Name, out instanceName);

            var performanceCounterInstance = cache.Get(instanceName ?? new CounterInstanceName(sp.Name, endpointName));

            sp.Register(() => performanceCounterInstance.Increment());
        }

        foreach (var dp in context.Durations)
        {
            var performanceCounterInstance = cache.Get(new CounterInstanceName(dp.Name, endpointName));
            dp.Register(d => performanceCounterInstance.RawValue = (long)d.TotalSeconds);
        }
    }

    readonly PerformanceCountersCache cache;
    readonly Dictionary<string, CounterInstanceName?> legacyInstanceNameMap;
    readonly string endpointName;
}