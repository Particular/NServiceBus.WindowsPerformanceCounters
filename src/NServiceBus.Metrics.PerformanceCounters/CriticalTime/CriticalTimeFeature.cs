using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Features;

class CriticalTimeFeature : Feature
{
    public const string CounterName = "Critical Time";
    protected override void Setup(FeatureConfigurationContext context)
    {
        context.ThrowIfSendonly();
//        var counterInstanceName = context.Settings.EndpointName();
        var criticalTimeCounter = new CriticalTimeCounter(null);
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
