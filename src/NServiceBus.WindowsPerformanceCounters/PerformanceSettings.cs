namespace NServiceBus
{
    using System;
    using Settings;

    public class PerformanceSettings
    {
        SettingsHolder settings;

        internal PerformanceSettings(SettingsHolder settings)
        {
            this.settings = settings;
        }

        public void EndpointSLATimeout(TimeSpan sla)
        {
            Guard.AgainstNegativeAndZero(nameof(sla), sla);
            settings.Set("WindowsPerformanceCountersSLATime", sla);
        }
    }
}