using System;
using System.Diagnostics;
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
        PerformanceCounter counter1;

        if (counterInstanceName.Length > sbyte.MaxValue)
        {
            throw new Exception($"The endpoint name ('{counterInstanceName}') is too long (longer then {sbyte.MaxValue}) to register as a performance counter instance name. Reduce the endpoint name.");
        }

        try
        {
            counter1 = new PerformanceCounter("NServiceBus", CounterName, counterInstanceName, false);
        }
        catch (Exception exception)
        {
            var message = $"NServiceBus performance counter for '{CounterName}' is not set up correctly. To rectify this problem, consult the NServiceBus performance counters documentation.";
            throw new Exception(message, exception);
        }
        var counter = new PerformanceCounterInstance(counter1);
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
