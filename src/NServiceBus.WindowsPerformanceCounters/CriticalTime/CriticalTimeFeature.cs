namespace NServiceBus.WindowsPerformanceCounters
{
    using System.Threading.Tasks;
    using Features;

    class CriticalTimeFeature : Feature
    {
        protected override void Setup(FeatureConfigurationContext context)
        {
            var counterInstanceName = context.Settings.EndpointName();
            var counter = PerformanceCounterHelper.InstantiatePerformanceCounter("Critical Time", counterInstanceName);
            var criticalTimeCounter = new CriticalTimeCounter(counter);
            var startup = new StartupTask(criticalTimeCounter);

            context.Pipeline.OnReceivePipelineCompleted(e =>
            {
                criticalTimeCounter.Update(e);
                return Task.FromResult(0);
            });

            context.RegisterStartupTask(() => startup);
        }

        class StartupTask : FeatureStartupTask
        {
            CriticalTimeCounter counter;

            public StartupTask(CriticalTimeCounter counter)
            {
                this.counter = counter;
            }

            protected override Task OnStart(IMessageSession session)
            {
                counter.Start();
                return Task.FromResult(0);
            }

            protected override Task OnStop(IMessageSession session)
            {
                counter.Dispose();
                return Task.FromResult(0);
            }

        }
    }
}