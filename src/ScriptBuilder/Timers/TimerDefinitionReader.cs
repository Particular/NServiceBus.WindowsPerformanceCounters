namespace NServiceBus.Metrics.PerformanceCounters
{
    using System.Collections.Generic;
    using System.Linq;
    using Mono.Cecil;

    static class TimerDefinitionReader
    {
        public static bool TryGetTimerDefinition(TypeDefinition type, out List<TimerDefinition> definitions)
        {
            definitions = new List<TimerDefinition>();
            if (type.BaseType != null && type.BaseType.FullName == "NServiceBus.Metrics.MetricBuilder")
            {
                var timerAttributes = type.Fields.Select(f => CecilExtentions.GetSingleAttribute(f, "NServiceBus.Metrics.TimerAttribute")).Where(c => c != null);

                foreach (var timerAttribute in timerAttributes)
                {
                    var name = (string) timerAttribute.ConstructorArguments[0].Value;
                    var unit = (string) timerAttribute.ConstructorArguments[1].Value;
                    var description = (string)timerAttribute.ConstructorArguments[2].Value;
                    var tags = (string[]) timerAttribute.ConstructorArguments[3].Value ?? new string[] {};

                    definitions.Add(new TimerDefinition(name, description, unit, tags));
                }
            }
            return definitions.Count > 0;
        }
    }
}