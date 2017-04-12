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
                var meterAttributes = type.Fields.Select(f => f.GetSingleAttribute("NServiceBus.Metrics.MeterAttribute")).Where(c => c != null);

                foreach (var meterAttribute in meterAttributes)
                {
                    var name = (string)meterAttribute.ConstructorArguments[0].Value;
                    var unit = (string)meterAttribute.ConstructorArguments[1].Value;
                    var description = (string)meterAttribute.ConstructorArguments[2].Value;
                    var tags = (string[])meterAttribute.ConstructorArguments[3].Value ?? new string[] { };
                    
                    definitions.Add(new MeterDefinition(name, description, unit, tags));
                }
            }
            return definitions.Count > 0;
        }
    }
}