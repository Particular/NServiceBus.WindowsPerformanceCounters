namespace NServiceBus
{
    using WindowsPerformanceCounters;

    public static class EndpointConfigurationExensionsForPerformanceCounters
    {
        public static PerformanceCounters EnableWindowsPerformanceCounters(this EndpointConfiguration endpointConfiguration)
        {
            Guard.AgainstNull(nameof(endpointConfiguration), endpointConfiguration);

#pragma warning disable 618
            endpointConfiguration.DisableFeature<NServiceBus.Features.ReceiveStatisticsPerformanceCounters>;
#pragma warning restore 618
            endpointConfiguration.EnableFeature<ReceiveStatisticsFeature>();
            endpointConfiguration.EnableFeature<CriticalTimeFeature>();

            return new PerformanceCounters(endpointConfiguration);
        }
    }
}