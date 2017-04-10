using NServiceBus.Features;

class PerformanceCountersFeature : Feature
{
    //MetricsOptions options;

    public PerformanceCountersFeature()
    {
        Defaults(s =>
        {
            //options= s.EnableMetrics();
        });
    }

    protected override void Setup(FeatureConfigurationContext context)
    {
        //options.EnableCustomReport(payload => { }, TimeSpan.FromSeconds(1));
    }


    public const string MessagesPulledPerSecondCounterName = "# of msgs pulled from the input queue /sec";
    public const string MessagesProcessedPerSecondCounterName = "# of msgs successfully processed / sec";
    public const string MessagesFailuresPerSecondCounterName = "# of msgs failures / sec";
}