namespace NServiceBus.Metrics.PerformanceCounters
{
    using System;
    using Mono.Cecil;

    public static class CounterWriter
    {
        public static void WriteScript(string scriptPath, ModuleDefinition module, Action<string, string> logError)
        {
            var timers = AllTimersDefinitionReader.GetTimers(module, (exception, type) =>
            {
                logError($"Error in '{type.FullName}'. Error:{exception.Message}", type.GetFileName());
            });

            var meters = AllMetersDefinitionReader.GetMeters(module, (exception, type) =>
            {
                logError($"Error in '{type.FullName}'. Error:{exception.Message}", type.GetFileName());
            });
        }
    }
}