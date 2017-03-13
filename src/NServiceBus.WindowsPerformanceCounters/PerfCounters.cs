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

        public PerformanceCounters EnableSLACounters(Action<PerformanceSettings> customizations)
        {
            customizations(new PerformanceSettings(this.GetSettings()));
            return this;
        }
    }

    public class PerformanceSettings
    {
        SettingsHolder settings;

        internal PerformanceSettings(SettingsHolder settings)
        {
            this.settings = settings;
        }

        public void EndpointSLATimeout(TimeSpan sla)
        {
            Guard.AgainstNull(nameof(sla), sla);
            settings.Set("WindowsPerformanceCountersSLATime", sla);
        }
    }
}
