namespace NServiceBus
{
    using System;
    using WindowsPerformanceCounters;
    using Configuration.AdvanceExtensibility;

    public static class MetricsExtensionsForPerformanceCounters
    {
        /// <summary>
        /// Add performance counter functionality to <see cref="EndpointConfiguration"/>.
        /// </summary>
        /// <param name="metrics">The <see cref="Metrics" /> instance to apply the settings to.</param>
        public static void EnableCriticalTimePerformanceCounter(this Metrics metrics)
        {
            Guard.AgainstNull(nameof(metrics), metrics);
            metrics.EndpointConfiguration.EnableFeature<CriticalTimeFeature>();
        }

        /// <summary>
        /// Enables the NServiceBus specific performance counters with a specific EndpointSLA.
        /// </summary>
        /// <param name="metrics">The <see cref="Metrics" /> instance to apply the settings to.</param>
        public static void EnableSLAPerformanceCounters(this Metrics metrics)
        {
            Guard.AgainstNull(nameof(metrics), metrics);
            metrics.EndpointConfiguration.EnableFeature<SLAMonitoringFeature>();
        }

        /// <summary>
        /// Enables the NServiceBus specific performance counters with a specific EndpointSLA.
        /// </summary>
        /// <param name="metrics">The <see cref="Metrics" /> instance to apply the settings to.</param>
        /// <param name="sla">The <see cref="TimeSpan" /> to use oa the SLA. Must be greater than <see cref="TimeSpan.Zero" />.</param>
        public static void EnableSLAPerformanceCounters(this Metrics metrics, TimeSpan sla)
        {
            Guard.AgainstNull(nameof(metrics), metrics);
            Guard.AgainstNegativeAndZero(nameof(sla), sla);

            metrics.EndpointConfiguration.GetSettings().Set(SLAMonitoringFeature.EndpointSLAKey, sla);
            EnableSLAPerformanceCounters(metrics);
        }

        /// <summary>
        /// Enables the NServiceBus statistics performance counters.
        /// </summary>
        /// <param name="metrics">The <see cref="Metrics" /> instance to apply the settings to.</param>
        public static void EnablePerformanceStatistics(this Metrics metrics)
        {
            Guard.AgainstNull(nameof(metrics), metrics);
            metrics.EndpointConfiguration.EnableFeature<ReceiveStatisticsFeature>();
        }
    }
}