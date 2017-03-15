namespace NServiceBus
{
    using WindowsPerformanceCounters;

    public static class EndpointConfigurationExensionsForPerformanceCounters
    {
        /// <summary>
        /// Enables statistical and criticaltime performance counters for this NServiceBus endpoint
        /// </summary>
        /// <param name="endpointConfiguration">The <see cref="EndpointConfiguration" /> instance to apply the settings to.</param>
        /// <returns></returns>
        public static PerformanceCounters EnableWindowsPerformanceCounters(this EndpointConfiguration endpointConfiguration)
        {
            Guard.AgainstNull(nameof(endpointConfiguration), endpointConfiguration);

#pragma warning disable 618
            endpointConfiguration.DisableFeature<Features.ReceiveStatisticsPerformanceCounters>();
#pragma warning restore 618
            endpointConfiguration.EnableFeature<ReceiveStatisticsFeature>();
            endpointConfiguration.EnableFeature<CriticalTimeFeature>();

            return new PerformanceCounters(endpointConfiguration);
        }
    }
}