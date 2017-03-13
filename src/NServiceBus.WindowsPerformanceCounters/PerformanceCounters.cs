namespace NServiceBus
{
    using System;
    using Configuration.AdvanceExtensibility;
    using Settings;

    public class PerformanceCounters : ExposeSettings
    {
        internal PerformanceCounters(SettingsHolder settings) : base(settings)
        {
        }

        public PerformanceCounters EnableSLACounters(Action<PerformanceSettings> customizations)
        {
            customizations(new PerformanceSettings(this.GetSettings()));
            return this;
        }
    }
}
