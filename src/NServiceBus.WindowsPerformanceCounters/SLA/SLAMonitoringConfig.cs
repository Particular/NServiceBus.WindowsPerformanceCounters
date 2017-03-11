namespace NServiceBus.WindowsPerformanceCounters
{
    using System;
    using Configuration.AdvanceExtensibility;

    /// <summary>
    /// Provide configuration options for monitoring related settings.
    /// </summary>
    public static class SLAMonitoringConfig
    {
        /// <summary>
        /// Enables the NServiceBus specific performance counters with a specific EndpointSLA.
        /// </summary>
        /// <param name="endpointConfiguration">The <see cref="EndpointConfiguration" /> instance to apply the settings to.</param>
        /// <param name="sla">The <see cref="TimeSpan" /> to use oa the SLA. Must be greater than <see cref="TimeSpan.Zero" />.</param>
        public static void EnableSLAPerformanceCounter(this EndpointConfiguration endpointConfiguration, TimeSpan sla)
        {
            Guard.AgainstNull(nameof(endpointConfiguration), endpointConfiguration);
            Guard.AgainstNegativeAndZero(nameof(sla), sla);
            endpointConfiguration.GetSettings().Set(SLAMonitoring.EndpointSLAKey, sla);
            EnableSLAPerformanceCounter(endpointConfiguration);
        }

        /// <summary>
        /// Enables the NServiceBus specific performance counters with a specific EndpointSLA.
        /// </summary>
        /// <param name="endpointConfiguration">The <see cref="EndpointConfiguration" /> instance to apply the settings to.</param>
        public static void EnableSLAPerformanceCounter(this EndpointConfiguration endpointConfiguration)
        {
            Guard.AgainstNull(nameof(endpointConfiguration), endpointConfiguration);
            endpointConfiguration.EnableFeature<SLAMonitoring>();
        }
    }
}