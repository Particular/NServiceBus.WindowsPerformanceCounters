namespace NServiceBus
{
    using System;
    using Configuration.AdvanceExtensibility;
    using Settings;

    public static class PerformanceCountersEndpointConfigurationExtensions
    {
        public static PerformanceCounters WindowsPerformanceCounters(this EndpointConfiguration config)
        {
            return new PerformanceCounters(config.GetSettings());
        }
    }

    public class PerformanceCounters : ExposeSettings
    {
        public PerformanceCounters(SettingsHolder settings) : base(settings)
        {
        }

        public PerformanceCounters EnableSLACounters(Action<PerfCountSettings> customizations)
        {
            customizations(new PerfCountSettings(this.GetSettings()));
            return this;
        }
    }

    public class PerfCountSettings : ExposeSettings
    {
        public PerfCountSettings(SettingsHolder settings) : base(settings)
        {
        }

        public void EndpointSLATimeout(TimeSpan sla)
        {
            Guard.AgainstNull(nameof(sla), sla);
            this.GetSettings().Set("WindowsPerformanceCountersSLATime", sla);


        }
    }
}
