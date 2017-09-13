namespace NServiceBus.Metrics.PerformanceCounters
{
    using System;
    using System.Collections.Generic;
    using Mono.Cecil;

    public static class CounterWriter
    {
        public static void WriteScript(string scriptPath, BuildScriptVariant variant, ModuleDefinition module, Action<string, string> logError)
        {
            var timers = AllDurationsDefinitionReader.GetDurations(module, (exception, type) =>
            {
                logError($"Error in '{type.FullName}'. Error:{exception.Message}", type.GetFileName());
            });

            var meters = AllSignalsDefinitionReader.GetSignals(module, (exception, type) =>
            {
                logError($"Error in '{type.FullName}'. Error:{exception.Message}", type.GetFileName());
            });

            switch (variant)
            {
                case BuildScriptVariant.CSharp:
                    CSharpCounterWriter.WriteCode(scriptPath, timers, meters, legacyInstanceNameMap);
                    break;
                case BuildScriptVariant.Powershell:
                    PowerShellCounterWriter.WriteScript(scriptPath, timers, meters, legacyInstanceNameMap);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(variant), variant, null);
            }
        }

        static Dictionary<string, string> legacyInstanceNameMap = new Dictionary<string, string>
        {
            {"# of message failures / sec", "# of msgs failures / sec"},
            {"# of messages pulled from the input queue / sec", "# of msgs pulled from the input queue /sec"},
            {"# of messages successfully processed / sec", "# of msgs successfully processed / sec"}
        };
    }
}