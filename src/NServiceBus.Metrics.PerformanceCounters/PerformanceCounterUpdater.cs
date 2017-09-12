using System;
using System.Collections.Generic;
using System.Diagnostics;
using NServiceBus;
using NServiceBus.Metrics.PerformanceCounters;

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
        foreach (var signalProbe in context.Signals)
        {
            legacyInstanceNameMap.TryGetValue(signalProbe.Name, out var instanceName);

            var performanceCounterInstance = cache.Get(instanceName ?? new CounterInstanceName(signalProbe.Name, endpointName));

            signalProbe.Register((ref SignalEvent e) => performanceCounterInstance.Increment());
        }

        foreach (var durationProbe in context.Durations)
        {
            if (durationProbe.Name == CounterNameConventions.ProcessingTime || durationProbe.Name == CounterNameConventions.CriticalTime)
            {
                var performanceCounterInstance = cache.Get(new CounterInstanceName(durationProbe.Name, endpointName));
                durationProbe.Register((ref DurationEvent d) => performanceCounterInstance.RawValue = (long) d.Duration.TotalSeconds);
            }

            var averageTimerCounter = cache.Get(new CounterInstanceName(durationProbe.Name.GetAverageTimerCounterName(), endpointName));
            var baseAverageTimerCounter = cache.Get(new CounterInstanceName(durationProbe.Name.GetAverageTimerBaseCounterName(), endpointName));

            durationProbe.Register((ref DurationEvent d) =>
            {
                var performanceCounterTicks = d.Duration.Ticks * Stopwatch.Frequency / TimeSpan.TicksPerSecond;
                averageTimerCounter.IncrementBy(performanceCounterTicks);
                baseAverageTimerCounter.Increment();
            });
        }
    }

    readonly PerformanceCountersCache cache;
    readonly Dictionary<string, CounterInstanceName?> legacyInstanceNameMap;
    readonly string endpointName;
}