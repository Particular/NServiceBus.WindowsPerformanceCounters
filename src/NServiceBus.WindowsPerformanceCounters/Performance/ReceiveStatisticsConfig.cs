namespace NServiceBus
{
    using WindowsPerformanceCounters;

    /// <summary>
    /// Provide configuration options for statistics performance counters.
    /// </summary>
    public static class ReceiveStatisticsConfig
    {
        /// <summary>
        /// Enables the NServiceBus statistics performance counters.
        /// </summary>
        /// <param name="endpointConfiguration">The <see cref="EndpointConfiguration" /> instance to apply the settings to.</param>
        public static void EnablePerformanceCounters(this EndpointConfiguration endpointConfiguration)
        {
            Guard.AgainstNull(nameof(endpointConfiguration), endpointConfiguration);
            endpointConfiguration.EnableFeature<ReceiveStatisticsFeature>();
        }
    }
}
