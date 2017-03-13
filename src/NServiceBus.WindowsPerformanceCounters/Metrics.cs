namespace NServiceBus
{
    using System;
    using WindowsPerformanceCounters;
    using Configuration.AdvanceExtensibility;

    public static class MetricsExtensions
    {
        public static Metrics Metrics(this EndpointConfiguration config)
        {
            Guard.AgainstNull(nameof(config), config);

            // This is bogus, but perhaps usable to verify it we access the extensionmethod via Metrics()
            config.GetSettings().Set("Metrics", "Activated");

            return new Metrics(config);
        }
    }

    public class Metrics : ExposeSettings
    {
        readonly EndpointConfiguration config;

        public Metrics(EndpointConfiguration config) : base(config.GetSettings())
        {
            this.config = config;
        }

        public EndpointConfiguration Config
        {
            get { return config; }
        }
    }

    public static class MetricsExtensionsForPerformanceCounters
    {
        /// <summary>
        /// Add performance counter functionality to <see cref="EndpointConfiguration"/>.
        /// </summary>
        /// <param name="metrics">The <see cref="Metrics" /> instance to apply the settings to.</param>
        public static Metrics EnableCriticalTimePerformanceCounter(this Metrics metrics)
        {
            Guard.AgainstNull(nameof(metrics), metrics);

            metrics.Config.EnableFeature<CriticalTimeMonitoring>();
            
            return metrics;
        }

        /// <summary>
        /// Enables the NServiceBus specific performance counters with a specific EndpointSLA.
        /// </summary>
        /// <param name="metrics">The <see cref="Metrics" /> instance to apply the settings to.</param>
        public static Metrics EnableSLAPerformanceCounters(this Metrics metrics)
        {
            Guard.AgainstNull(nameof(metrics), metrics);

            metrics.Config.EnableFeature<SLAMonitoring>();

            return metrics;
        }

        /// <summary>
        /// Enables the NServiceBus specific performance counters with a specific EndpointSLA.
        /// </summary>
        /// <param name="metrics">The <see cref="Metrics" /> instance to apply the settings to.</param>
        /// <param name="sla">The <see cref="TimeSpan" /> to use oa the SLA. Must be greater than <see cref="TimeSpan.Zero" />.</param>
        public static Metrics EnableSLAPerformanceCounters(this Metrics metrics, TimeSpan sla)
        {
            Guard.AgainstNull(nameof(metrics), metrics);
            Guard.AgainstNegativeAndZero(nameof(sla), sla);

            metrics.GetSettings().Set(SLAMonitoring.EndpointSLAKey, sla);
            EnableSLAPerformanceCounters(metrics);

            return metrics;
        }

        /// <summary>
        /// Enables the NServiceBus statistics performance counters.
        /// </summary>
        /// <param name="metrics">The <see cref="Metrics" /> instance to apply the settings to.</param>
        public static Metrics EnablePerformanceStatistics(this Metrics metrics)
        {
            Guard.AgainstNull(nameof(metrics), metrics);

            metrics.Config.EnableFeature<ReceiveStatisticsFeature>();

            return metrics;
        }
    }
}
