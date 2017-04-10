namespace NServiceBus
{
    using System.Collections.Generic;
    using Metrics.PerformanceCounters.Counters;

    class PerformanceCounterUpdater
    {
        public PerformanceCounterUpdater(PerformanceCountersCache cache)
        {
            this.cache = cache;
        }

        public void Update(string payload)
        {
            var rootObject = Newtonsoft.Json.Linq.JObject.Parse(payload);

            var context = rootObject.Value<string>("Context");
            var meters = rootObject["Meters"].ToObject<List<Meter>>();
            foreach (var meter in meters)
            {
                var performanceCounterInstance = cache.Get(new CounterInstanceName(meter.Name, context));
                performanceCounterInstance.RawValue = meter.Count;
            }
        }

        readonly PerformanceCountersCache cache;
    }
}