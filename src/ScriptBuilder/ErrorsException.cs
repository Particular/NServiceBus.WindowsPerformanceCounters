namespace NServiceBus.Metrics.PerformanceCounters
{
    using System;

    public class ErrorsException : Exception
    {
        public ErrorsException(string message) : base(message)
        {
        }

        public string FileName { get; set; }
    }
}