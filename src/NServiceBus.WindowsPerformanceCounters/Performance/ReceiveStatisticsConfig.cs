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
        /// <param name="config">The <see cref="EndpointConfiguration" /> instance to apply the settings to.</param>
        public static void EnablePerformanceCounters(this EndpointConfiguration config)
        {
            Guard.AgainstNull(nameof(config), config);
            config.EnableFeature<ReceiveStatisticsFeature>();
        }
    }
}
