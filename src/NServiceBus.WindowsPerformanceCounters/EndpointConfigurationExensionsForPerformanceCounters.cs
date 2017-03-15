namespace NServiceBus
{
    using WindowsPerformanceCounters;

    public static class EndpointConfigurationExensionsForPerformanceCounters
    {
        public static PerformanceCounters EnableWindowsPerformanceCounters(this EndpointConfiguration endpointConfiguration)
        {
            Guard.AgainstNull(nameof(endpointConfiguration), endpointConfiguration);

            endpointConfiguration.EnableFeature<ReceiveStatisticsFeature>();
            endpointConfiguration.EnableFeature<CriticalTimeFeature>();

            return new PerformanceCounters(endpointConfiguration);
        }
    }
}