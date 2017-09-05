namespace NServiceBus.Metrics.PerformanceCounters
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    static class CSharpCounterWriter
    {
        public static void WriteCode(string scriptPath, IEnumerable<DurationDefinition> durations, IEnumerable<SignalDefinition> signals, Dictionary<string, string> legacyInstanceNameMap)
        {
            var outputPath = Path.Combine(scriptPath, "Counters.g.cs");
            using (var streamWriter = File.CreateText(outputPath))
            {
                var stringBuilder = new StringBuilder();

                var slaCounterDefinition = $@"new CounterCreationData(""SLA violation countdown"", ""{CounterNameAndDescriptionConventions.LegacySlaViolationCounterDescription}"", PerformanceCounterType.NumberOfItems32),";
                stringBuilder.AppendLine(slaCounterDefinition.PadLeft(slaCounterDefinition.Length + 8));

                foreach (var duration in durations)
                {
                    var averageTimerName = duration.Name.GetAverageTimerCounterName();
                    var averageTimerBase = duration.Name.GetAverageTimerBaseCounterName();
                    var averageTimerDescription = CounterNameAndDescriptionConventions.GetAverageTimerDescription(duration.Name, duration.Description);

                    var durationAverageDefinition = $@"new CounterCreationData(""{averageTimerName}"", ""{averageTimerDescription}"", PerformanceCounterType.AverageTimer32),";
                    stringBuilder.AppendLine(durationAverageDefinition.PadLeft(durationAverageDefinition.Length + 8));

                    var durationBaseDefinition = $@"new CounterCreationData(""{averageTimerBase}"", ""{averageTimerDescription}"", PerformanceCounterType.AverageBase),";
                    stringBuilder.AppendLine(durationBaseDefinition.PadLeft(durationBaseDefinition.Length + 8));

                    if (duration.Name == CounterNameAndDescriptionConventions.ProcessingTime || duration.Name == CounterNameAndDescriptionConventions.CriticalTime)
                    {
                        var legacyDescription = duration.Name == CounterNameAndDescriptionConventions.ProcessingTime
                            ? CounterNameAndDescriptionConventions.LegacyProcessingTimeDescription
                            : CounterNameAndDescriptionConventions.LegacyCriticalTimeDescription;

                        var legacyTimerDefinition = $@"new CounterCreationData(""{duration.Name}"", ""{legacyDescription}"", PerformanceCounterType.NumberOfItems32),";
                        stringBuilder.AppendLine(legacyTimerDefinition.PadLeft(legacyTimerDefinition.Length + 8));
                    }
                }

                foreach (var signal in signals)
                {
                    string instanceName;
                    legacyInstanceNameMap.TryGetValue(signal.Name, out instanceName);

                    var signalDefinition = $@"new CounterCreationData(""{instanceName ?? signal.Name}"", ""{signal.Description}"", PerformanceCounterType.RateOfCountsPerSecond32),";
                    stringBuilder.AppendLine(signalDefinition.PadLeft(signalDefinition.Length + 8));
                }

                streamWriter.Write(Template, stringBuilder);
            }
        }

        const string Template = @"using System;
using System.Diagnostics;
using System.Security;
using System.Runtime.CompilerServices;

[CompilerGenerated]
public static class CounterCreator 
{{
    public static void Create() 
    {{
        var counterCreationCollection = new CounterCreationDataCollection(Counters);
        try
        {{
            var categoryName = ""NServiceBus"";
            if (PerformanceCounterCategory.Exists(categoryName))
            {{
                PerformanceCounterCategory.Delete(categoryName);
            }}

            PerformanceCounterCategory.Create(
                categoryName: categoryName,
                categoryHelp: ""NServiceBus statistics"",
                categoryType: PerformanceCounterCategoryType.MultiInstance,
                counterData: counterCreationCollection);
            PerformanceCounter.CloseSharedResources();
        }} catch(Exception ex) when(ex is SecurityException || ex is UnauthorizedAccessException)
        {{
            throw new Exception(""Execution requires elevated permissions"", ex);
        }}
    }}

    static CounterCreationData[] Counters = new CounterCreationData[]
    {{
{0}
    }};
}}";
    }
}
