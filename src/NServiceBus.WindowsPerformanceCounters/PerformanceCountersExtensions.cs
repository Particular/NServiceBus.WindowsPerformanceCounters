namespace NServiceBus
{
    using Configuration.AdvanceExtensibility;

    public static class PerformanceCountersExtensions
    {
        public static PerformanceCounters WindowsPerformanceCounters(this EndpointConfiguration endpointConfiguration)
        {
            Guard.AgainstNull(nameof(endpointConfiguration), endpointConfiguration);
            return new PerformanceCounters(endpointConfiguration.GetSettings());
        }
    }
}