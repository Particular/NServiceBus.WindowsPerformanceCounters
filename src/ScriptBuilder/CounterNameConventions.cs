namespace NServiceBus.Metrics.PerformanceCounters
{
    static class CounterNameConventions
    {
        public static string GetAverageTimerCounterName(this string durationName)
        {
            return $"{durationName} Average";
        }

        public static string GetAverageTimerBaseCounterName(this string durationName)
        {
            return $"{durationName} AverageBase";
        }

        public const string ProcessingTime = "Processing Time";
        public const string CriticalTime = "Critical Time";
    }
}