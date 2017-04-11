using System.Collections.Generic;
using NServiceBus;
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
        var logicalAddress = context.Settings.LogicalAddress();

        // ReSharper disable once UnusedVariable
        var legacyInstanceNameMap = new Dictionary<string, CounterInstanceName?>
        {
            { "# of message failures / sec", new CounterInstanceName(MessagesFailuresPerSecondCounterName, logicalAddress.EndpointInstance.Endpoint) },
            { "# of messages pulled from the input queue / sec", new CounterInstanceName(MessagesPulledPerSecondCounterName, logicalAddress.EndpointInstance.Endpoint) },
            { "# of messages successfully processed / sec", new CounterInstanceName(MessagesProcessedPerSecondCounterName, logicalAddress.EndpointInstance.Endpoint) },
        };
    }
    
    public const string MessagesPulledPerSecondCounterName = "# of msgs pulled from the input queue /sec";
    public const string MessagesProcessedPerSecondCounterName = "# of msgs successfully processed / sec";
    public const string MessagesFailuresPerSecondCounterName = "# of msgs failures / sec";
}