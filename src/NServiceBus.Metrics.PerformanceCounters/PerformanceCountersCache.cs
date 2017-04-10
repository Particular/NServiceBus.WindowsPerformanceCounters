namespace NServiceBus
{
    using System.Collections.Concurrent;

    class PerformanceCountersCache
    {
        public IPerformanceCounterInstance Get(CounterInstanceName counterInstanceName)
        {
            var counter = counterCache.GetOrAdd(counterInstanceName, CreateInstance);

            return counter;
        }

        protected virtual IPerformanceCounterInstance CreateInstance(CounterInstanceName counterInstanceName)
        {
            return PerformanceCounterHelper.InstantiatePerformanceCounter(counterInstanceName.CounterName, counterInstanceName.InstanceName);
        }

        ConcurrentDictionary<CounterInstanceName, IPerformanceCounterInstance> counterCache = new ConcurrentDictionary<CounterInstanceName, IPerformanceCounterInstance>();
    }
}