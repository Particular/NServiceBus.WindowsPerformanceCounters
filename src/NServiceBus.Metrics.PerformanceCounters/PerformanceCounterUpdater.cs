using System;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json.Linq;

class PerformanceCounterUpdater
{
    public PerformanceCounterUpdater(PerformanceCountersCache cache, Dictionary<string, CounterInstanceName?> legacyInstanceNameMap, TimeSpan? resetTimersAfter = null)
    {
        resetAfterTicks = (resetTimersAfter ?? TimeSpan.FromSeconds(2)).Ticks;
        this.legacyInstanceNameMap = legacyInstanceNameMap;
        this.cache = cache;

        // initialize to an armed state
        OnReceivePipelineCompleted();
    }

    public void Update(string payload)
    {
        var rootObject = JObject.Parse(payload);

        var context = rootObject.Value<string>("Context");
        var meters = rootObject["Meters"]?.ToObject<List<Meter>>() ?? new List<Meter>();
        foreach (var meter in meters)
        {
            CounterInstanceName? instanceName;
            legacyInstanceNameMap.TryGetValue(meter.Name, out instanceName);

            var performanceCounterInstance = cache.Get(instanceName ?? new CounterInstanceName(meter.Name, context));
            performanceCounterInstance.RawValue = meter.Count;
        }

        var timers = rootObject["Timers"]?.ToObject<List<Timer>>() ?? new List<Timer>();
        foreach (var timer in timers)
        {
            var performanceCounterInstance = cache.Get(new CounterInstanceName(timer.Name, context));

            var idleFor = NowTicks - Volatile.Read(ref lastCompleted);
            if (idleFor > resetAfterTicks)
            {
                performanceCounterInstance.RawValue = 0;
            }
            else
            {
                performanceCounterInstance.RawValue = timer.Histogram.LastValue / 1000;
            }
        }
    }

    readonly PerformanceCountersCache cache;
    readonly Dictionary<string, CounterInstanceName?> legacyInstanceNameMap;
    long lastCompleted;

    public void OnReceivePipelineCompleted()
    {
        Volatile.Write(ref lastCompleted, NowTicks);
    }

    static long NowTicks => DateTime.UtcNow.Ticks;
    readonly long resetAfterTicks;
}