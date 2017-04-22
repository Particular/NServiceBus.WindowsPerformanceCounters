namespace NServiceBus.Metrics.PerformanceCounters
{
    using System;
    using System.Collections.Generic;
    using Mono.Cecil;

    static class AllTimersDefinitionReader
    {
        public static IEnumerable<TimerDefinition> GetTimers(ModuleDefinition module, Action<ErrorsException, TypeDefinition> logError)
        {
            var timerDefinitions = new List<TimerDefinition>();
            foreach (var type in module.AllClasses())
            {
                try
                {
                    List<TimerDefinition> definition;
                    if (TimerDefinitionReader.TryGetTimerDefinition(type, out definition))
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