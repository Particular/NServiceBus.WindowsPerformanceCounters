namespace NServiceBus
{
    using System.Collections.Generic;
    using Metrics.PerformanceCounters.Counters;
    using Newtonsoft.Json.Linq;

    class PerformanceCounterUpdater
    {
        public PerformanceCounterUpdater(PerformanceCountersCache cache, Dictionary<string, CounterInstanceName?> legacyInstanceNameMap)
        {
            this.legacyInstanceNameMap = legacyInstanceNameMap;
            this.cache = cache;
        }

        public void Update(string payload)
        {
            var rootObject = JObject.Parse(payload);

            var context = rootObject.Value<string>("Context");
            var meters = rootObject["Meters"].ToObject<List<Meter>>();
            foreach (var meter in meters)
            {
                CounterInstanceName? instanceName;
                legacyInstanceNameMap.TryGetValue(meter.Name, out instanceName);

                var performanceCounterInstance = cache.Get(instanceName ?? new CounterInstanceName(meter.Name, context));
                performanceCounterInstance.RawValue = meter.Count;
            }
        }

        readonly PerformanceCountersCache cache;
        readonly Dictionary<string, CounterInstanceName?> legacyInstanceNameMap;
    }
}