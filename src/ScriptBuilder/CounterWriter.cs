namespace NServiceBus.Metrics.PerformanceCounters
{
    using System;
    using System.Collections.Generic;
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

            CSharpCounterWriter.WriteCode(scriptPath, timers, meters, legacyInstanceNameMap);
        }

        static Dictionary<string, string> legacyInstanceNameMap = new Dictionary<string, string>
        {
            {"# of message failures / sec", "# of msgs failures / sec"},
            {"# of messages pulled from the input queue / sec", "# of msgs pulled from the input queue /sec"},
            {"# of messages successfully processed / sec", "# of msgs successfully processed / sec"}
        };
    }
}