namespace NServiceBus
{
    public class Metrics
    {
        internal Metrics(EndpointConfiguration endpointConfiguration)
        {
            EndpointConfiguration = endpointConfiguration;
        }

        internal EndpointConfiguration EndpointConfiguration { get; }
    }
}