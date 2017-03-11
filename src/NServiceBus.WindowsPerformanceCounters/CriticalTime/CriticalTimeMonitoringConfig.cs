namespace NServiceBus
{
    using WindowsPerformanceCounters;

    /// <summary>
    /// Add performance counter functionality to <see cref="EndpointConfiguration"/>.
    /// </summary>
    public static class CriticalTimeMonitoringConfig
    {
        /// <summary>
        /// Enables the NServiceBus specific performance counters.
        /// </summary>
        /// <param name="endpointConfiguration">The <see cref="EndpointConfiguration" /> instance to apply the settings to.</param>
        public static void EnableCriticalTimePerformanceCounter(this EndpointConfiguration endpointConfiguration)
        {
            Guard.AgainstNull(nameof(endpointConfiguration), endpointConfiguration);
            endpointConfiguration.EnableFeature<CriticalTimeMonitoring>();
        }
    }
}
