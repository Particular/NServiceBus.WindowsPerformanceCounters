namespace NServiceBus.Metrics.PerformanceCounters
{
    static class CounterNameAndDescriptionConventions
    {
        public static string GetAverageTimerCounterName(this string durationName)
        {
            return $"Avg. {durationName}";
        }

        public static string GetAverageTimerBaseCounterName(this string durationName)
        {
            return $"Avg. {durationName}Base";
        }

        public static string GetAverageTimerDescription(string durationName, string durationDescription)
        {
            return $"{durationName.GetAverageTimerCounterName()} - {durationDescription} The value is the average duration in seconds during the sample interval.";
        }

        public const string ProcessingTime = "Processing Time";
        public const string CriticalTime = "Critical Time";

        public const string LegacySlaViolationCounterDescription = "SLA Violation Countdown - Seconds until the SLA for this endpoint is breached. The value is in rounded seconds. This is an instantaneous snapshot, not an average over the time interval.";
        public const string LegacyProcessingTimeDescription = "Processing time - The time it took to successfully process a message. The value is in rounded seconds. This is an instantaneous snapshot, not an average over the time interval.";
        public const string LegacyCriticalTimeDescription = "Critical time - The time it took from sending to processing the message. The value is in rounded seconds. This is an instantaneous snapshot, not an average over the time interval.";
    }
}