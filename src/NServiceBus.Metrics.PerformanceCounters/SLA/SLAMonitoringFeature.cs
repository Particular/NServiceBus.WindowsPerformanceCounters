using System;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Features;

class SLAMonitoringFeature : Feature
{
    protected override void Setup(FeatureConfigurationContext context)
    {
        context.ThrowIfSendOnly();
        var settings = context.Settings;
        var endpointSla = settings.Get<TimeSpan>(EndpointSLAKey);

        var counterInstanceName = settings.EndpointName();

        cache = new PerformanceCountersCache();
        var counter = cache.Get(new CounterInstanceName(CounterName, counterInstanceName));

        slaBreachCounter = new EstimatedTimeToSLABreachCounter(endpointSla, counter);

        context.Pipeline.OnReceivePipelineCompleted((pipelineCompleted, _) =>
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

        protected override Task OnStart(IMessageSession session, CancellationToken cancellationToken = default)
        {
            feature.slaBreachCounter.Start();
            return Task.CompletedTask;
        }

        protected override Task OnStop(IMessageSession session, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        SLAMonitoringFeature feature;
    }
}