namespace NServiceBus.Metrics.PerformanceCounters
{
    using System;
    using System.Collections.Generic;
    using Mono.Cecil;

    static class AllSignalsDefinitionReader
    {
        public static IEnumerable<SignalDefinition> GetSignals(ModuleDefinition module, Action<ErrorsException, TypeDefinition> logError)
        {
            var timerDefinitions = new List<SignalDefinition>();
            foreach (var type in module.AllClasses())
            {
                try
                {
                    if (SignalDefinitionReader.TryGetSignalDefinition(type, out var definition))
                    {
                        timerDefinitions.AddRange(definition);
                    }
                }
                catch (ErrorsException exception)
                {
                    logError(exception, type);
                }
            }
            return timerDefinitions;
        }
    }
}