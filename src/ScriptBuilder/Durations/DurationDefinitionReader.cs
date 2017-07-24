namespace NServiceBus.Metrics.PerformanceCounters
{
    using System.Collections.Generic;
    using System.Linq;
    using Mono.Cecil;

    static class DurationDefinitionReader
    {
        public static bool TryGetDurationDefinition(TypeDefinition type, out List<DurationDefinition> definitions)
        {
            definitions = new List<DurationDefinition>();
            if (type.BaseType != null && type.BaseType.FullName == "DurationProbeBuilder")
            {
                var attributes = type.CustomAttributes.Where(a => a.AttributeType.FullName == "ProbePropertiesAttribute");

                foreach (var attribute in attributes)
                {
                    var name = attribute.ParameterValue<string>("name");
                    var description = attribute.ParameterValue<string>("description");

                    definitions.Add(new DurationDefinition(name, description));
                }
            }
            return definitions.Count > 0;
        }
    }
}