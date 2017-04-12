namespace NServiceBus.Metrics.PerformanceCounters
{
    class MeterDefinition
    {
        public MeterDefinition(string name, string unit, string[] tags = null)
        {
            Name = name;
            Unit = unit;
            Tags = tags;
        }

        public string Name { get; }

        public string Unit { get; }

        public string[] Tags { get; }
    }
}