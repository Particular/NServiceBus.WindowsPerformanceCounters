using System.Collections.Generic;
using System.Linq;
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
        context.Signals.ToList().ForEach(sp =>
        {
            CounterInstanceName? instanceName;
            legacyInstanceNameMap.TryGetValue(sp.Name, out instanceName);

            var performanceCounterInstance = cache.Get(instanceName ?? new CounterInstanceName(sp.Name, endpointName));

            sp.Register(() => performanceCounterInstance.RawValue += 1);
        });

        context.Durations.ToList().ForEach(dp =>
        {
            var performanceCounterInstance = cache.Get(new CounterInstanceName(dp.Name, endpointName));

            dp.Register(d => performanceCounterInstance.RawValue = (long)d.TotalSeconds);
        });
    }

    readonly PerformanceCountersCache cache;
    readonly Dictionary<string, CounterInstanceName?> legacyInstanceNameMap;
    readonly string endpointName;
}