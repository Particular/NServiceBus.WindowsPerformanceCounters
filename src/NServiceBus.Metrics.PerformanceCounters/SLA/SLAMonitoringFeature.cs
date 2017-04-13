using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Features;
using NServiceBus.Metrics.PerformanceCounters.Counters;

class SLAMonitoringFeature : Feature
{
    protected override void Setup(FeatureConfigurationContext context)
    {
        context.ThrowIfSendonly();
        var settings = context.Settings;
        var endpointSla = settings.Get<TimeSpan>(EndpointSLAKey);

        var counterInstanceName = settings.EndpointName();

        cache = new PerformanceCountersCache();
        var counter = cache.Get(new CounterInstanceName(CounterName, counterInstanceName));

        slaBreachCounter = new EstimatedTimeToSLABreachCounter(endpointSla, counter);

        context.Pipeline.OnReceivePipelineCompleted(pipelineCompleted =>
        {
            slaBreachCounter?.Update(pipelineCompleted);
            return Task.FromResult(0);
        });

        context.RegisterStartupTask(new StartupTask(this));
    }

    PerformanceCountersCache cache;
    EstimatedTimeToSLABreachCounter slaBreachCounter;

    public const string CounterName = "SLA violation countdown";


    internal const string EndpointSLAKey = "EndpointSLA";

    class StartupTask : FeatureStartupTask, IDisposable
    {
        public StartupTask(SLAMonitoringFeature feature)
        {
            this.feature = feature;
        }

        public void Dispose()
        {
            feature.slaBreachCounter?.Dispose();
            feature.slaBreachCounter = null;
            feature.cache?.Dispose();
        }

        protected override Task OnStart(IMessageSession session)
        {
            feature.slaBreachCounter.Start();
            return TaskExtensions.CompletedTask;
        }

        protected override Task OnStop(IMessageSession session)
        {
            return TaskExtensions.CompletedTask;
        }

        SLAMonitoringFeature feature;
    }
}