using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Features;
using NServiceBus.Metrics.PerformanceCounters.Counters;

class PerformanceCountersFeature : Feature
{
    MetricsOptions options;

    public PerformanceCountersFeature()
    {
        Defaults(s =>
        {
            options = s.EnableMetrics();
        });
    }

    protected override void Setup(FeatureConfigurationContext context)
    {
        var logicalAddress = context.Settings.LogicalAddress();

        // ReSharper disable once UnusedVariable
        var legacyInstanceNameMap = new Dictionary<string, CounterInstanceName?>
        {
            { "# of message failures / sec", new CounterInstanceName(MessagesFailuresPerSecondCounterName, logicalAddress.EndpointInstance.Endpoint) },
            { "# of messages pulled from the input queue / sec", new CounterInstanceName(MessagesPulledPerSecondCounterName, logicalAddress.EndpointInstance.Endpoint) },
            { "# of messages successfully processed / sec", new CounterInstanceName(MessagesProcessedPerSecondCounterName, logicalAddress.EndpointInstance.Endpoint) },
        };

        var cache = new PerformanceCountersCache();
        var updater = new PerformanceCounterUpdater(cache, legacyInstanceNameMap);

        options.EnableCustomReport(payload =>
        {
            updater.Update(payload);
            return CompletedTask;
        }, TimeSpan.FromSeconds(2));
    }

    static Task CompletedTask = Task.FromResult(0);

    public const string MessagesPulledPerSecondCounterName = "# of msgs pulled from the input queue /sec";
    public const string MessagesProcessedPerSecondCounterName = "# of msgs successfully processed / sec";
    public const string MessagesFailuresPerSecondCounterName = "# of msgs failures / sec";
    public const string CriticalTimeCounterName = "Critical Time";
}