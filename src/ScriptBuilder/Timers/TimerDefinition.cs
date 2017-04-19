namespace NServiceBus.Metrics.PerformanceCounters
{
    class TimerDefinition
    {
        public TimerDefinition(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public readonly string Description;

        public readonly string Name;
    }
}