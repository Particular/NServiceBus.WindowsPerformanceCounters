namespace NServiceBus.Metrics.PerformanceCounters
{
    class DurationDefinition
    {
        public DurationDefinition(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public readonly string Description;

        public readonly string Name;
    }
}