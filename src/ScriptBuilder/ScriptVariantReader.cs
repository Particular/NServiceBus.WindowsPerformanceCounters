using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace NServiceBus.Metrics.PerformanceCounters
{
    public static class ScriptVariantReader
    {
        public static IEnumerable<BuildScriptVariant> Read(ModuleDefinition moduleDefinition)
        {
            var attribute = moduleDefinition.Assembly.CustomAttributes
                .FirstOrDefault(x => x.AttributeType.FullName == "NServiceBus.Metrics.PerformanceCounters.PerformanceCounterSettingsAttribute");
            if (attribute == null)
            {
                yield return BuildScriptVariant.CSharp;
                yield return BuildScriptVariant.Powershell;
                yield break;
            }

            var csharpScripts = attribute.GetBoolProperty("CSharp");
            if (csharpScripts)
            {
                yield return BuildScriptVariant.CSharp;
            }

            var powerShellScripts = attribute.GetBoolProperty("Powershell");
            if (powerShellScripts)
            {
                yield return BuildScriptVariant.Powershell;
            }

            if (!csharpScripts && !powerShellScripts)
            {
                throw new ErrorsException("Must define either CSharpScripts, PowershellScripts, or both. Add a [PerformanceCounterSettingsAttribute] to the assembly.");
            }
        }
    }
}