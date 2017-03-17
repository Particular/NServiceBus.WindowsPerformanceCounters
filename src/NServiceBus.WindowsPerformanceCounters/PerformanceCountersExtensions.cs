namespace NServiceBus
{

    /// <summary>
    /// Exposes windows performance counters configuration on <see cref="EndpointConfiguration"/>.
    /// </summary>
    public static class PerformanceCountersExtensions
    {
        /// <summary>
        /// Enables receive statistics and critical time performance counters.
        /// </summary>
        /// <param name="endpointConfiguration">The <see cref="EndpointConfiguration" /> instance to apply the settings to.</param>
        public static PerformanceCountersSettings EnableWindowsPerformanceCounters(this EndpointConfiguration endpointConfiguration)
        {
            Guard.AgainstNull(nameof(endpointConfiguration), endpointConfiguration);

#pragma warning disable 618
            endpointConfiguration.DisableFeature<Features.ReceiveStatisticsPerformanceCounters>();
#pragma warning restore 618
            endpointConfiguration.EnableFeature<ReceiveStatisticsFeature>();
            endpointConfiguration.EnableFeature<CriticalTimeFeature>();

            return new PerformanceCountersSettings(endpointConfiguration);
        }
    }
}