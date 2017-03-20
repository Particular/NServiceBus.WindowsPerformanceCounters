using System;
using System.Diagnostics;

static class PerformanceCounterHelper
{
    public static PerformanceCounterInstance InstantiatePerformanceCounter(string counterName, string instanceName)
    {
        PerformanceCounter counter;

        if (instanceName.Length > sbyte.MaxValue)
        {
            throw new Exception($"The endpoint name ('{instanceName}') is too long (longer then {sbyte.MaxValue}) to register as a performance counter instance name. Reduce the endpoint name.");
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
        return new PerformanceCounterInstance(counter);
    }
}