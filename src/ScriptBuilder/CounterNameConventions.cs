namespace NServiceBus.Metrics.PerformanceCounters
{
    static class CounterNameConventions
    {
        public static string GetAverageTimerCounterName(this string durationName)
        {
            return $"Avg. {durationName} (sec)";
        }

        public static string GetAverageTimerBaseCounterName(this string durationName)
        {
            return $"Avg. {durationName} (sec)Base";
        }

        public const string ProcessingTime = "Processing Time";
        public const string CriticalTime = "Critical Time";
    }
}