using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Features;

class PerformanceCountersFeature : Feature
{
    public PerformanceCountersFeature()
    {
        Defaults(s =>
        {
            options = s.EnableMetrics();
        });
    }

    protected override void Setup(FeatureConfigurationContext context)
    {
        var endpoint = context.Settings.LogicalAddress().EndpointInstance.Endpoint;

        var legacyInstanceNameMap = new Dictionary<string, CounterInstanceName?>
        {
            {"# of message failures / sec", new CounterInstanceName(MessagesFailuresPerSecondCounterName, endpoint)},
            {"# of messages pulled from the input queue / sec", new CounterInstanceName(MessagesPulledPerSecondCounterName, endpoint)},
            {"# of messages successfully processed / sec", new CounterInstanceName(MessagesProcessedPerSecondCounterName, endpoint)}
        };

        cache = new PerformanceCountersCache();
        updater = new PerformanceCounterUpdater(cache, legacyInstanceNameMap, endpoint);

        context.RegisterStartupTask(new Cleanup(this));

        options.RegisterObservers(probeContext =>
        {
            updater.Observe(probeContext);
        });
    }

    MetricsOptions options;
    PerformanceCounterUpdater updater;
    PerformanceCountersCache cache;

    public const string MessagesPulledPerSecondCounterName = "# of msgs pulled from the input queue /sec";
    public const string MessagesProcessedPerSecondCounterName = "# of msgs successfully processed / sec";
    public const string MessagesFailuresPerSecondCounterName = "# of msgs failures / sec";
    public const string CriticalTimeCounterName = "Critical Time";
    public const string ProcessingTimeCounterName = "Processing Time";

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