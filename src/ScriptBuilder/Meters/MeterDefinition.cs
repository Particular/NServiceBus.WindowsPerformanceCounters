namespace NServiceBus.Metrics.PerformanceCounters
{
    class MeterDefinition
    {
        public MeterDefinition(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public readonly string Description;

        public readonly string Name;
    }
}