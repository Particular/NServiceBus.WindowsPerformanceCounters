namespace NServiceBus.Metrics.PerformanceCounters
{
    using System;
    using System.Collections.Generic;
    using Mono.Cecil;

    static class AllDurationsDefinitionReader
    {
        public static IEnumerable<DurationDefinition> GetDurations(ModuleDefinition module, Action<ErrorsException, TypeDefinition> logError)
        {
            var timerDefinitions = new List<DurationDefinition>();
            foreach (var type in module.AllClasses())
            {
                try
                {
                    List<DurationDefinition> definition;
                    if (DurationDefinitionReader.TryGetDurationDefinition(type, out definition))
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