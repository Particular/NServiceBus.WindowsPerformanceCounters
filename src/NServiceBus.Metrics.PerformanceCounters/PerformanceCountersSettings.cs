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
        /// Sets the update interval.
        /// </summary>
        /// <param name="updateInterval"></param>
        [ObsoleteEx(Message = "This interval is no longer used for reporting. Counters values are updated as soon as they are reported", RemoveInVersion = "3.0")]
        public void UpdateCounterEvery(TimeSpan updateInterval)
        {
        }
    }
}