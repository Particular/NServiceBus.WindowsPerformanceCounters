namespace NServiceBus.Metrics.PerformanceCounters
{
    using System.Collections.Generic;
    using System.Linq;
    using Mono.Cecil;

    static class SignalDefinitionReader
    {
        public static bool TryGetSignalDefinition(TypeDefinition type, out List<SignalDefinition> definitions)
        {
            definitions = new List<SignalDefinition>();
            if (type.BaseType != null && type.BaseType.FullName == "SignalProbeBuilder")
            {
                var attributes = type.CustomAttributes.Where(a => a.AttributeType.FullName == "ProbePropertiesAttribute");

                foreach (var attribute in attributes)
                {
                    var name = attribute.ParameterValue<string>("name");
                    var description = attribute.ParameterValue<string>("description");
                    definitions.Add(new SignalDefinition(name, description));
                }
            }
            return definitions.Count > 0;
        }
    }
}