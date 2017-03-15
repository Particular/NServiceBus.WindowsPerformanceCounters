namespace NServiceBus
{
    public class PerformanceCounters
    {
        internal PerformanceCounters(EndpointConfiguration endpointConfiguration)
        {
            EndpointConfiguration = endpointConfiguration;
        }

        internal EndpointConfiguration EndpointConfiguration { get; }
    }
}