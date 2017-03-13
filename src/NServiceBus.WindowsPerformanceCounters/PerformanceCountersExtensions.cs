namespace NServiceBus
{
    using Configuration.AdvanceExtensibility;

    public static class PerformanceCountersExtensions
    {
        public static PerformanceCounters WindowsPerformanceCounters(this EndpointConfiguration config)
        {
            return new PerformanceCounters(config.GetSettings());
        }
    }
}