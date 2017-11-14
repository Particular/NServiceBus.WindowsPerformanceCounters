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

            endpointConfiguration.EnableFeature<PerformanceCountersFeature>();

            return new PerformanceCountersSettings(endpointConfiguration);
        }
    }
}