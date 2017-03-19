using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Features;

class SLAMonitoringFeature : Feature
{
   public  const string CounterName = "SLA violation countdown";
    protected override void Setup(FeatureConfigurationContext context)
    {
        context.ThrowIfSendonly();
        var settings = context.Settings;
        var endpointSla = settings.Get<TimeSpan>(EndpointSLAKey);

        var counterInstanceName = settings.EndpointName();
        var counter = PerformanceCounterHelper.InstantiatePerformanceCounter(CounterName, counterInstanceName);
        var slaBreachCounter = new EstimatedTimeToSLABreachCounter(endpointSla, counter);
        var startup = new StartupTask(slaBreachCounter);

        context.Pipeline.OnReceivePipelineCompleted(pipelineCompleted =>
        {
            slaBreachCounter.Update(pipelineCompleted);
            return Task.FromResult(0);
        });

        context.RegisterStartupTask(() => startup);
    }


    internal const string EndpointSLAKey = "EndpointSLA";

    class StartupTask : FeatureStartupTask
    {
        public StartupTask(EstimatedTimeToSLABreachCounter slaBreachCounter)
        {
            this.slaBreachCounter = slaBreachCounter;
        }

        protected override Task OnStart(IMessageSession session)
        {
            slaBreachCounter.Start();
            return Task.FromResult(0);
        }

        protected override Task OnStop(IMessageSession session)
        {
            slaBreachCounter.Dispose();
            return Task.FromResult(0);
        }

        EstimatedTimeToSLABreachCounter slaBreachCounter;
    }
}
