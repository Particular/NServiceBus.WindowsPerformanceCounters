namespace NServiceBus
{
    using System;
    using Configuration.AdvancedExtensibility;

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
#if NETSTANDARD
            if (!System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                throw new PlatformNotSupportedException("Windows Performance Counters are not supported on this platform.");
            }
#endif
            Guard.AgainstNegativeAndZero(nameof(sla), sla);

            endpointConfiguration.GetSettings().Set(SLAMonitoringFeature.EndpointSLAKey, sla);
            endpointConfiguration.EnableFeature<SLAMonitoringFeature>();
        }
    }
}