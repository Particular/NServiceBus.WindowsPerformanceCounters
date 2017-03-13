namespace NServiceBus
{
    using Configuration.AdvanceExtensibility;

    public static class PerformanceCountersEndpointConfigurationExtensions
    {
        public static PerformanceCounters WindowsPerformanceCounters(this EndpointConfiguration config)
        {
            return new PerformanceCounters(config.GetSettings());
        }
    }
}