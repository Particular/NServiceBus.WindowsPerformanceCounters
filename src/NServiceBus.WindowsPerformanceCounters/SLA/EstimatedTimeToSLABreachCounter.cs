namespace NServiceBus.WindowsPerformanceCounters
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    class EstimatedTimeToSLABreachCounter : IDisposable
    {
        public EstimatedTimeToSLABreachCounter(TimeSpan endpointSla, IPerformanceCounterInstance counter)
        {
            this.endpointSla = endpointSla;
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
            var dataPoint = new DataPoint(processingEnded - sent, processingEnded, processingEnded - processingStarted);

            lock (dataPoints)
            {
                dataPoints.Add(dataPoint);
                if (dataPoints.Count > MaxDataPoints)
                {
                    dataPoints.RemoveRange(0, dataPoints.Count - MaxDataPoints);
                }
            }

            UpdateTimeToSLABreach();
        }

        public void Start()
        {
            timer = new Timer(RemoveOldDataPoints, null, 0, 2000);
        }

        void UpdateTimeToSLABreach()
        {
            List<DataPoint> snapshots;

            lock (dataPoints)
            {
                snapshots = new List<DataPoint>(dataPoints);
            }

            var secondsToSLABreach = CalculateTimeToSLABreach(snapshots);

            counter.RawValue = Convert.ToInt32(Math.Min(secondsToSLABreach, int.MaxValue));
        }

        double CalculateTimeToSLABreach(List<DataPoint> snapshots)
        {
            DataPoint? first = null;
            DataPoint? previous = null;

            var criticalTimeDelta = TimeSpan.Zero;

            foreach (var current in snapshots)
            {
                if (!first.HasValue)
                {
                    first = current;
                }

                if (previous.HasValue)
                {
                    criticalTimeDelta += current.CriticalTime - previous.Value.CriticalTime;
                }

                previous = current;
            }

            if (criticalTimeDelta.TotalSeconds <= 0.0)
            {
                return double.MaxValue;
            }

            var elapsedTime = previous.Value.OccurredAt - first.Value.OccurredAt;

            if (elapsedTime.TotalSeconds <= 0.0)
            {
                return double.MaxValue;
            }

            var lastKnownCriticalTime = previous.Value.CriticalTime.TotalSeconds;

            var criticalTimeDeltaPerSecond = criticalTimeDelta.TotalSeconds / elapsedTime.TotalSeconds;

            var secondsToSLABreach = (endpointSla.TotalSeconds - lastKnownCriticalTime) / criticalTimeDeltaPerSecond;

            if (secondsToSLABreach < 0.0)
            {
                return 0.0;
            }

            return secondsToSLABreach;
        }

        void RemoveOldDataPoints(object state)
        {
            lock (dataPoints)
            {
                var last = dataPoints.Count == 0 ? default(DataPoint?) : dataPoints[dataPoints.Count - 1];

                if (last.HasValue)
                {
                    var oldestDataToKeep = DateTime.UtcNow - new TimeSpan(last.Value.ProcessingTime.Ticks * 3);

                    dataPoints.RemoveAll(d => d.OccurredAt < oldestDataToKeep);
                }
            }

            UpdateTimeToSLABreach();
        }
        public void Dispose()
        {
            timer?.Dispose();
            counter?.Dispose();
        }

        IPerformanceCounterInstance counter;
        List<DataPoint> dataPoints = new List<DataPoint>();
        TimeSpan endpointSla;
        Timer timer;

        const int MaxDataPoints = 10;
    }
}