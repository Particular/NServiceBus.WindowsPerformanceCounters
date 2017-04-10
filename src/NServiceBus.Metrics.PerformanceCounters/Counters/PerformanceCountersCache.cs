namespace NServiceBus.Metrics.PerformanceCounters.Counters
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;

    class PerformanceCountersCache : IDisposable
    {
        public IPerformanceCounterInstance Get(CounterInstanceName counterInstanceName)
        {
            var counter = counterCache.GetOrAdd(counterInstanceName, CreateInstance);

            return counter;
        }

        public void Dispose()
        {
            foreach (var counter in counterCache.Values)
            {
                counter.Dispose();
            }

            counterCache.Clear();
        }

        protected virtual IPerformanceCounterInstance CreateInstance(CounterInstanceName counterInstanceName)
        {
            PerformanceCounter counter;

            if (counterInstanceName.InstanceName.Length > sbyte.MaxValue)
            {
                throw new Exception($"The endpoint name ('{counterInstanceName.InstanceName}') is too long (longer then {sbyte.MaxValue}) to register as a performance counter instance name. Reduce the endpoint name.");
            }

            try
            {
                counter = new PerformanceCounter("NServiceBus", counterInstanceName.CounterName, counterInstanceName.InstanceName, false);
            }
            catch (Exception exception)
            {
                var message = $"NServiceBus performance counter for '{counterInstanceName.CounterName}' is not set up correctly. To rectify this problem, consult the NServiceBus performance counters documentation.";
                throw new Exception(message, exception);
            }
            return new PerformanceCounterInstance(counter);
        }

        ConcurrentDictionary<CounterInstanceName, IPerformanceCounterInstance> counterCache = new ConcurrentDictionary<CounterInstanceName, IPerformanceCounterInstance>();
    }
}