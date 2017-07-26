namespace NServiceBus.Metrics.PerformanceCounters
{
    static class CounterNameConventions
    {
        public static string GetAverageTimerCounterName(this string durationName)
        {
            return $"Avg. {durationName}";
        }

        public static string GetAverageTimerBaseCounterName(this string durationName)
        {
            return $"Avg. {durationName}Base";
        }

        public const string ProcessingTime = "Processing Time";
        public const string CriticalTime = "Critical Time";
    }
}