using System;
using System.Diagnostics;
using NServiceBus;

class CriticalTimeCounter : IDisposable
{
    public CriticalTimeCounter(IPerformanceCounterInstance counter)
    {
        this.counter = counter;
    }

    public void Update(ReceivePipelineCompleted completed)
    {
        DateTime timeSent;
        if (completed.TryGetTimeSent(out timeSent))
        {
            Update(timeSent, completed.StartedAt, completed.CompletedAt);
        }
    }

    public void Update(DateTime sent, DateTime processingStarted, DateTime processingEnded)
    {
        var endToEndTime = processingEnded - sent;
        counter.RawValue = Convert.ToInt32(endToEndTime.TotalSeconds);

        lastMessageProcessedTime = processingEnded;

        var processingDuration = processingEnded - processingStarted;
        estimatedMaximumProcessingDuration = processingDuration.Add(TimeSpan.FromSeconds(1));
        Trace.WriteLine(estimatedMaximumProcessingDuration);
    }

    public void Start()
    {
        timer = new System.Threading.Timer(ResetCounterValueIfNoMessageHasBeenProcessedRecently, null, 0, 2000);
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
        counter?.Dispose();
    }

    IPerformanceCounterInstance counter;
    TimeSpan estimatedMaximumProcessingDuration = TimeSpan.FromSeconds(2);
    DateTime lastMessageProcessedTime;
    System.Threading.Timer timer;

}