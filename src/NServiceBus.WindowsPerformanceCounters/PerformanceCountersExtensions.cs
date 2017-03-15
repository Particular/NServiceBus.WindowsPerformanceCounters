namespace NServiceBus
{
    using System;
    using WindowsPerformanceCounters;
    using Configuration.AdvanceExtensibility;

    public static class PerformanceCountersExtensions
    {
        /// <summary>
        /// Enables the NServiceBus specific performance counters with a specific EndpointSLA.
        /// </summary>
        /// <param name="performanceCounters">The <see cref="PerformanceCounters" /> instance to apply the settings to.</param>
        public static void EnableSLAPerformanceCounters(this PerformanceCounters performanceCounters)
        {
            Guard.AgainstNull(nameof(performanceCounters), performanceCounters);
            performanceCounters.EndpointConfiguration.EnableFeature<SLAMonitoringFeature>();
        }

        /// <summary>
        /// Enables the NServiceBus specific performance counters with a specific EndpointSLA.
        /// </summary>
        /// <param name="performanceCounters">The <see cref="PerformanceCounters" /> instance to apply the settings to.</param>
        /// <param name="sla">The <see cref="TimeSpan" /> to use oa the SLA. Must be greater than <see cref="TimeSpan.Zero" />.</param>
        public static void EnableSLAPerformanceCounters(this PerformanceCounters performanceCounters, TimeSpan sla)
        {
            Guard.AgainstNull(nameof(performanceCounters), performanceCounters);
            Guard.AgainstNegativeAndZero(nameof(sla), sla);

            performanceCounters.EndpointConfiguration.GetSettings().Set(SLAMonitoringFeature.EndpointSLAKey, sla);
            EnableSLAPerformanceCounters(performanceCounters);
        }
    }
}