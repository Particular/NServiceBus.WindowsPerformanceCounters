namespace NServiceBus
{
    using System;
    using Configuration.AdvanceExtensibility;

    /// <summary>
    /// Windows performance counter configuration instance.
    /// </summary>
    public class PerformanceCountersSettings
    {
        EndpointConfiguration endpointConfiguration;

        internal PerformanceCountersSettings(EndpointConfiguration endpointConfiguration)
        {
            this.endpointConfiguration = endpointConfiguration;
        }

        /// <summary>
        /// Enables the Time To Breach SLA performance counter.
        /// </summary>
        /// <param name="sla">The SLA to use. Must be greater than <see cref="TimeSpan.Zero" />.</param>
        public void EnableSLAPerformanceCounters(TimeSpan sla)
        {
            Guard.AgainstNegativeAndZero(nameof(sla), sla);

            endpointConfiguration.GetSettings().Set(SLAMonitoringFeature.EndpointSLAKey, sla);
            endpointConfiguration.EnableFeature<SLAMonitoringFeature>();
        }

        /// <summary>
        /// Defines the update interval for the performance counter values.
        /// </summary>
        /// <param name="updateInterval">The update interval.</param>
        public void UpdateCounterEvery(TimeSpan updateInterval)
        {
            Guard.AgainstNegativeAndZero(nameof(updateInterval), updateInterval);

            endpointConfiguration.GetSettings().Set(PerformanceCountersFeature.UpdateIntervalKey, updateInterval);
        }
    }
}