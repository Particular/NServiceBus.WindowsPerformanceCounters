namespace NServiceBus.Metrics.PerformanceCounters
{
    class SignalDefinition
    {
        public SignalDefinition(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public readonly string Description;

        public readonly string Name;
    }
}