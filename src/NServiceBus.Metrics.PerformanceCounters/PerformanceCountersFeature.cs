using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Features;
using NServiceBus.Metrics.PerformanceCounters.Counters;

class PerformanceCountersFeature : Feature
{
    public PerformanceCountersFeature()
    {
        Defaults(s =>
        {
            options = s.EnableMetrics();
            s.SetDefault(UpdateIntervalKey, TimeSpan.FromSeconds(2));
        });
    }

    protected override void Setup(FeatureConfigurationContext context)
    {
        var logicalAddress = context.Settings.LogicalAddress();

        var legacyInstanceNameMap = new Dictionary<string, CounterInstanceName?>
        {
            {"# of message failures / sec", new CounterInstanceName(MessagesFailuresPerSecondCounterName, logicalAddress.EndpointInstance.Endpoint)},
            {"# of messages pulled from the input queue / sec", new CounterInstanceName(MessagesPulledPerSecondCounterName, logicalAddress.EndpointInstance.Endpoint)},
            {"# of messages successfully processed / sec", new CounterInstanceName(MessagesProcessedPerSecondCounterName, logicalAddress.EndpointInstance.Endpoint)}
        };

        cache = new PerformanceCountersCache();
        updater = new PerformanceCounterUpdater(cache, legacyInstanceNameMap);

        context.RegisterStartupTask(new Cleanup(this));

        options.EnableCustomReport(payload =>
        {
            updater?.Update(payload);
            return TaskExtensions.CompletedTask;
        }, context.Settings.Get<TimeSpan>(UpdateIntervalKey));
    }

    MetricsOptions options;
    PerformanceCounterUpdater updater;
    PerformanceCountersCache cache;

    public const string MessagesPulledPerSecondCounterName = "# of msgs pulled from the input queue /sec";
    public const string MessagesProcessedPerSecondCounterName = "# of msgs successfully processed / sec";
    public const string MessagesFailuresPerSecondCounterName = "# of msgs failures / sec";
    public const string CriticalTimeCounterName = "Critical Time";
    public const string UpdateIntervalKey = "PerformanceCounterUpdateInterval";

    class Cleanup : FeatureStartupTask, IDisposable
    {
        public Cleanup(PerformanceCountersFeature feature)
        {
            this.feature = feature;
        }

        public void Dispose()
        {
            feature.updater = null;
            feature.cache.Dispose();
        }

        protected override Task OnStart(IMessageSession session)
        {
            return TaskExtensions.CompletedTask;
        }

        protected override Task OnStop(IMessageSession session)
        {
            return TaskExtensions.CompletedTask;
        }

        PerformanceCountersFeature feature;
    }
}