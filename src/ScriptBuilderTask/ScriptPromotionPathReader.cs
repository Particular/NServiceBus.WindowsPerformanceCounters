namespace NServiceBus.Metrics.PerformanceCounters
{
    using System.Linq;
    using Mono.Cecil;

    public static class ScriptPromotionPathReader
    {
        public static bool TryRead(ModuleDefinition moduleDefinition, out string target)
        {
            var customAttribute = moduleDefinition.Assembly.CustomAttributes
                .FirstOrDefault(x => x.AttributeType.FullName == "NServiceBus.Metrics.PerformanceCounters.PerformanceCounterSettingsAttribute");
            if (customAttribute == null)
            {
                target = null;
                return false;
            }

            target = customAttribute.GetStringProperty("ScriptPromotionPath");
            if (target == null)
            {
                return false;
            }
            if (!string.IsNullOrWhiteSpace(target))
            {
                return true;
            }
            throw new ErrorsException("PerformanceCounterSettingsAttribute contains an empty ScriptPromotionPath.");
        }

    }
}