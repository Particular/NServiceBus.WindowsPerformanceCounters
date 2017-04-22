namespace NServiceBus.Metrics.PerformanceCounters
{
    using System;
    using System.Collections.Generic;
    using Mono.Cecil;

    static class AllMetersDefinitionReader
    {
        public static IEnumerable<MeterDefinition> GetMeters(ModuleDefinition module, Action<ErrorsException, TypeDefinition> logError)
        {
            var timerDefinitions = new List<MeterDefinition>();
            foreach (var type in module.AllClasses())
            {
                try
                {
                    List<MeterDefinition> definition;
                    if (MeterDefinitionReader.TryGetMeterDefinition(type, out definition))
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