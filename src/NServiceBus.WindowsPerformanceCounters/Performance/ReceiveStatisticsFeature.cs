using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Features;

class ReceiveStatisticsFeature : Feature
{
    protected override void Setup(FeatureConfigurationContext context)
    {
        context.ThrowIfSendonly();

        var logicalAddress = context.Settings.LogicalAddress();
        var performanceDiagnosticsBehavior = new ReceivePerformanceDiagnosticsBehavior(logicalAddress.EndpointInstance.Endpoint);

        context.Pipeline.Register("NServiceBus.WindowsPerformanceCounters.ReceivePerformanceDiagnosticsBehavior", performanceDiagnosticsBehavior, "Provides various performance counters for receive statistics");
        context.RegisterStartupTask(new WarmupCooldownTask(performanceDiagnosticsBehavior));
    }

    class WarmupCooldownTask : FeatureStartupTask
    {
        public WarmupCooldownTask(ReceivePerformanceDiagnosticsBehavior behavior)
        {
            this.behavior = behavior;
        }

        protected override Task OnStart(IMessageSession session)
        {
            behavior.Warmup();
            return Task.FromResult(0);
        }

        protected override Task OnStop(IMessageSession session)
        {
            behavior.Cooldown();
            return Task.FromResult(0);
        }

        ReceivePerformanceDiagnosticsBehavior behavior;
    }
}