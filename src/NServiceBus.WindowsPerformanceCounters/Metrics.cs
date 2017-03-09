namespace NServiceBus
{
    using WindowsPerformanceCounters;
    using Configuration.AdvanceExtensibility;

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

    public static class MetricsExtensions
    {
        public static Metrics EnableMetrics(this EndpointConfiguration config)
        {
            Guard.AgainstNull(nameof(config), config);

            // This is bogus, but perhaps usable to verify it we access the extensionmethod via EnableMetrics()
            config.GetSettings().Set("Metrics", "Activated");

            return new Metrics(config);
        }
    }

    public static class MetricsExtensionsForPerformanceCounters
    {
        public static Metrics EnableSLAPerformanceCounters(this Metrics metrics)
        {
            Guard.AgainstNull(nameof(metrics), metrics);

            metrics.Config.EnableFeature<SLAMonitoring>();

            return metrics;
        }

        /// <summary>
        /// Enables the NServiceBus statistics performance counters.
        /// </summary>
        /// <param name="metrics">The <see cref="Metrics" /> instance to apply the settings to.</param>
        public static Metrics EnableStatistics(this Metrics metrics)
        {
            Guard.AgainstNull(nameof(metrics), metrics);

            metrics.Config.EnableFeature<ReceiveStatisticsFeature>();

            return metrics;
        }

        /// <summary>
        /// Enables the NServiceBus statistics performance counters.
        /// </summary>
        /// <param name="metrics">The <see cref="Metrics" /> instance to apply the settings to.</param>
        public static Metrics EnablePerformanceCounters(this Metrics metrics)
        {
            Guard.AgainstNull(nameof(metrics), metrics);

            metrics.Config.EnableFeature<ReceiveStatisticsFeature>();

            return metrics;
        }
    }
}
