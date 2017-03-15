namespace NServiceBus
{
    using WindowsPerformanceCounters;

    public static class MetricsExtensions
    {
        public static Metrics EnableWindowsPerformanceCounters(this EndpointConfiguration endpointConfiguration)
        {
            Guard.AgainstNull(nameof(endpointConfiguration), endpointConfiguration);

            endpointConfiguration.EnableFeature<ReceiveStatisticsFeature>();
            endpointConfiguration.EnableFeature<CriticalTimeFeature>();

            return new Metrics(endpointConfiguration);
        }
    }
}