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
                var timerAttributes = type.Fields.Select(f => f.GetSingleAttribute("NServiceBus.Metrics.TimerAttribute")).Where(c => c != null);

                foreach (var timerAttribute in timerAttributes)
                {
                    var name = timerAttribute.ParameterValue<string>("name");
                    var description = timerAttribute.ParameterValue<string>("description");

                    definitions.Add(new TimerDefinition(name, description));
                }
            }
            return definitions.Count > 0;
        }
    }
}