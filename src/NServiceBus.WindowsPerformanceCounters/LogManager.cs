using System;
using NServiceBus.Logging;

static class LogManager
{
    public static ILog GetLogger<T>()
    {
        return GetLogger(typeof(T));
    }

    public static ILog GetLogger(Type type)
    {
        if (type.Namespace == null)
        {
            var name = $"NServiceBus.WindowsPerformanceCounters.{type.Name}";
            return NServiceBus.Logging.LogManager.GetLogger(name);
        }
        return NServiceBus.Logging.LogManager.GetLogger(type);
    }
}