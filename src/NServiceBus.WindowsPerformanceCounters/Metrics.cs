namespace NServiceBus
{
    using Configuration.AdvanceExtensibility;

    public static class MetricsExtensions
    {
        public static Metrics Metrics(this EndpointConfiguration endpointConfiguration)
        {
            Guard.AgainstNull(nameof(endpointConfiguration), endpointConfiguration);

            // This is bogus, but perhaps usable to verify it we access the extensionmethod via Metrics()
            endpointConfiguration.GetSettings().Set("Metrics", "Activated");

            return new Metrics(endpointConfiguration);
        }
    }

    public class Metrics
    {
        internal Metrics(EndpointConfiguration endpointConfiguration)
        {
            EndpointConfiguration = endpointConfiguration;
        }

        internal EndpointConfiguration EndpointConfiguration { get; }
    }
}
