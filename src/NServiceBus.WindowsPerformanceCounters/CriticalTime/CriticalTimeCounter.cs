namespace NServiceBus.WindowsPerformanceCounters
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    class CriticalTimeCounter : IDisposable
    {
        public CriticalTimeCounter(IPerformanceCounterInstance counter)
        {
            this.counter = counter;
        }

        public void Update(ReceivePipelineCompleted completed)
        {
            var headers = completed.ProcessedMessage.Headers;
            string timeSentString;
            if (headers.TryGetValue(Headers.TimeSent, out timeSentString))
            {
                var timeSent = DateTimeExtensions.ToUtcDateTime(timeSentString);
                Update(timeSent, completed.StartedAt, completed.CompletedAt);
            }
        }

        public void Update(DateTime sentInstant, DateTime processingStarted, DateTime processingEnded)
        {
            var endToEndTime = processingEnded - sentInstant;
            counter.RawValue = Convert.ToInt32(endToEndTime.TotalSeconds);

            lastMessageProcessedTime = processingEnded;

            var processingDuration = processingEnded - processingStarted;
            estimatedMaximumProcessingDuration = processingDuration.Add(TimeSpan.FromSeconds(1));
            Trace.WriteLine(estimatedMaximumProcessingDuration);
        }

        public void Start()
        {
            timer = new Timer(ResetCounterValueIfNoMessageHasBeenProcessedRecently, null, 0, 2000);
        }

        void ResetCounterValueIfNoMessageHasBeenProcessedRecently(object state)
        {
            if (NoMessageHasBeenProcessedRecently())
            {
                counter.RawValue = 0;
            }
        }

        bool NoMessageHasBeenProcessedRecently()
        {
            var timeFromLastMessageProcessed = DateTime.UtcNow - lastMessageProcessedTime;
            return timeFromLastMessageProcessed > estimatedMaximumProcessingDuration;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }

        IPerformanceCounterInstance counter;
        TimeSpan estimatedMaximumProcessingDuration = TimeSpan.FromSeconds(2);
        DateTime lastMessageProcessedTime;
        Timer timer;

    }
}