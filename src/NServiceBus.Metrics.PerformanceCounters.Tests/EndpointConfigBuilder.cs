using NServiceBus;

static class EndpointConfigBuilder
{
    public static EndpointConfiguration BuildEndpoint(string s)
    {
        var endpointConfiguration = new EndpointConfiguration(s);
        endpointConfiguration.UseTransport<LearningTransport>();
        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.UsePersistence<LearningPersistence>();
        var recoverability = endpointConfiguration.Recoverability();
        recoverability.Immediate(c => c.NumberOfRetries(0));
        recoverability.Delayed(c => c.NumberOfRetries(0));
        return endpointConfiguration;
    }
}