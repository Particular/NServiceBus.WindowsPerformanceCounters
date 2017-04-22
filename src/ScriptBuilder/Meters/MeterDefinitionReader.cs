namespace NServiceBus.Metrics.PerformanceCounters
{
    using System.Collections.Generic;
    using System.Linq;
    using Mono.Cecil;

    static class MeterDefinitionReader
    {
        public static bool TryGetMeterDefinition(TypeDefinition type, out List<MeterDefinition> definitions)
        {
            definitions = new List<MeterDefinition>();
            if (type.BaseType != null && type.BaseType.FullName == "NServiceBus.Metrics.MetricBuilder")
            {
                var attributes = type.Fields
                    .Select(f => f.GetSingleAttribute("NServiceBus.Metrics.MeterAttribute"))
                    .Where(c => c != null);

                foreach (var attribute in attributes)
                {
                    var name = attribute.ParameterValue<string>("name");
                    var description = attribute.ParameterValue<string>("description");
                    definitions.Add(new MeterDefinition(name, description));
                }
            }
            return definitions.Count > 0;
        }
    }
}