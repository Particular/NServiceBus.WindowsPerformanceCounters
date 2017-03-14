namespace NServiceBus.WindowsPerformanceCounters
{
    using System;
    using System.Threading.Tasks;
    using Features;

    class SLAMonitoringFeature : Feature
    {
        protected override void Setup(FeatureConfigurationContext context)
        {
            var settings = context.Settings;
            if (settings.GetOrDefault<bool>("Endpoint.SendOnly"))
            {
                throw new Exception("SLA Monitoring is not supported for send only endpoints, remove .EnableSLAPerformanceCounter(mySLA).");
            }

            TimeSpan endpointSla;

            if (!settings.TryGet(EndpointSLAKey, out endpointSla))
            {
                throw new Exception("Endpoint SLA is required for the `SLA violation countdown` counter. Pass the SLA for this endpoint to .EnableSLAPerformanceCounter(mySLA).");
            }

            var counterInstanceName = settings.EndpointName();
            var counter = PerformanceCounterHelper.InstantiatePerformanceCounter("SLA violation countdown", counterInstanceName);
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
}