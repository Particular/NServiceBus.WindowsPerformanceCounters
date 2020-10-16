using System;

struct DataPoint
{
    public DataPoint(TimeSpan criticalTime, DateTimeOffset occurredAt, TimeSpan processingTime)
    {
        CriticalTime = criticalTime;
        OccurredAt = occurredAt;
        ProcessingTime = processingTime;
    }

    public readonly TimeSpan CriticalTime;

    public readonly DateTimeOffset OccurredAt;

    public readonly TimeSpan ProcessingTime;
}