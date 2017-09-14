namespace NServiceBus.Metrics.PerformanceCounters
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    static class PowerShellCounterWriter
    {
        public static void WriteScript(string scriptPath, IEnumerable<DurationDefinition> durations, IEnumerable<SignalDefinition> signals, Dictionary<string, string> legacyInstanceNameMap)
        {
            var outputPath = Path.Combine(scriptPath, "CreateNSBPerfCounters.ps1");
            using (var streamWriter = File.CreateText(outputPath))
            {
                var stringBuilder = new StringBuilder();

                var slaCounterDefinition = @"New-Object System.Diagnostics.CounterCreationData ""SLA violation countdown"", ""Seconds until the SLA for this endpoint is breached."",  NumberOfItems32";
                stringBuilder.AppendLine(slaCounterDefinition.PadLeft(slaCounterDefinition.Length + 8));

                foreach (var duration in durations)
                {
                    var averageTimerName = duration.Name.GetAverageTimerCounterName();
                    var averageTimerBase = duration.Name.GetAverageTimerBaseCounterName();

                    var durationAverageDefinition = $@"New-Object System.Diagnostics.CounterCreationData ""{averageTimerName}"", ""{duration.Description}"",  AverageTimer32";
                    stringBuilder.AppendLine(durationAverageDefinition.PadLeft(durationAverageDefinition.Length + 8));

                    var durationBaseDefinition = $@"New-Object System.Diagnostics.CounterCreationData ""{averageTimerBase}"", ""{duration.Description}"",  AverageBase";
                    stringBuilder.AppendLine(durationBaseDefinition.PadLeft(durationBaseDefinition.Length + 8));
                    
                    if (duration.Name == CounterNameConventions.ProcessingTime || duration.Name == CounterNameConventions.CriticalTime)
                    {
                        var legacyTimerDefinition = $@"New-Object System.Diagnostics.CounterCreationData ""{duration.Name}"", ""{duration.Description}"",  NumberOfItems32";
                        stringBuilder.AppendLine(legacyTimerDefinition.PadLeft(legacyTimerDefinition.Length + 8));
                    }
                }

                foreach (var signal in signals)
                {
                    string instanceName;
                    legacyInstanceNameMap.TryGetValue(signal.Name, out instanceName);

                    var signalDefinition = $@"New-Object System.Diagnostics.CounterCreationData ""{instanceName ?? signal.Name}"", ""{signal.Description}"",  RateOfCountsPerSecond32";
                    stringBuilder.AppendLine(signalDefinition.PadLeft(signalDefinition.Length + 8));
                }

                streamWriter.Write(Template, stringBuilder);
            }
        }

        const string Template = @"
#requires -RunAsAdministrator
Function InstallNSBPerfCounters {{
    
    $category = @{{Name=""NServiceBus""; Description=""NServiceBus statistics""}}
    $counters = New-Object System.Diagnostics.CounterCreationDataCollection
    $counters.AddRange(@(
{0}
    ))

    if ([System.Diagnostics.PerformanceCounterCategory]::Exists($category.Name)) {{

       foreach($counter in $counters){{
            $exists = [System.Diagnostics.PerformanceCounterCategory]::CounterExists($counter.CounterName, $category.Name)
            if (!$exists){{
                Write-Host ""One or more counters are missing.The performance counter category will be recreated""
                [System.Diagnostics.PerformanceCounterCategory]::Delete($category.Name)

                break
            }}
        }}
    }}

    if (![System.Diagnostics.PerformanceCounterCategory]::Exists($category.Name)) {{
        Write-Host ""Creating the performance counter category""
        [void] [System.Diagnostics.PerformanceCounterCategory]::Create($category.Name, $category.Description, [System.Diagnostics.PerformanceCounterCategoryType]::MultiInstance, $counters)
        }}
    else {{
        Write-Host ""No performance counters have to be created""
    }}

    [System.Diagnostics.PerformanceCounter]::CloseSharedResources()
}}
InstallNSBPerfCounters
";
    }
}