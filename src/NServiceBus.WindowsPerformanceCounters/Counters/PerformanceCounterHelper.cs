namespace NServiceBus
{
    using System;
    using System.Diagnostics;
    using Logging;

    static class PerformanceCounterHelper
    {
        public static PerformanceCounterInstance InstantiatePerformanceCounter(string counterName, string instanceName)
        {
            PerformanceCounter counter;

            TryToInstantiatePerformanceCounter(counterName, instanceName, out counter);

            return new PerformanceCounterInstance(counter);
        }

        private static bool TryToInstantiatePerformanceCounter(string counterName, string instanceName, out PerformanceCounter counter)
        {
            if (instanceName.Length > 128)
            {
                throw new Exception($"The endpoint name ('{instanceName}') is too long (longer then {(int) sbyte.MaxValue}) to register as a performance counter instance name. Reduce the endpoint name.");
            }

            try
            {
                counter = new PerformanceCounter("NServiceBus", counterName, instanceName, false);
            }
            catch (Exception exception)
            {
                var message = $"NServiceBus performance counter for '{counterName}' is not set up correctly. To rectify this problem, consult the NServiceBus performance counters documentation.";
                throw new Exception(message, exception);
            }
            logger.DebugFormat("'{0}' counter initialized for '{1}'", counterName, instanceName);
            return true;
        }

        static ILog logger = LogManager.GetLogger(typeof(PerformanceCounterHelper));
    }
}