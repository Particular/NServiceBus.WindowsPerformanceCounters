namespace NServiceBus.WindowsPerformanceCounters
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Features;

    class SLAMonitoring : Feature
    {
        protected override void Setup(FeatureConfigurationContext context)
        {
            var settings = context.Settings;
            if (settings.GetOrDefault<bool>("Endpoint.SendOnly"))
            {
                throw new Exception("SLA Monitoring is not supported for send only endpoints, please remove .EnableSLAPerformanceCounter(mySLA) from your config.");
            }

            TimeSpan endpointSla;

            if (!settings.TryGet(EndpointSLAKey, out endpointSla))
            {
                throw new Exception("Endpoint SLA is required for the `SLA violation countdown` counter. Pass the SLA for this endpoint to .EnableSLAPerformanceCounter(mySLA).");
            }

            var counterInstanceName = settings.EndpointName();
            var slaBreachCounter = new EstimatedTimeToSLABreachCounter(endpointSla, counterInstanceName);

            context.Pipeline.OnReceivePipelineCompleted(e =>
            {
                string timeSentString;

                if (!e.ProcessedMessage.Headers.TryGetValue(Headers.TimeSent, out timeSentString))
                {
                    return Task.FromResult(0);
                }

                slaBreachCounter.Update(DateTimeExtensions.ToUtcDateTime(timeSentString), e.StartedAt, e.CompletedAt);

                return Task.FromResult(0);
            });

            context.RegisterStartupTask(() => slaBreachCounter);
        }

        internal const string EndpointSLAKey = "EndpointSLA";

        class EstimatedTimeToSLABreachCounter : FeatureStartupTask
        {
            public EstimatedTimeToSLABreachCounter(TimeSpan endpointSla, string counterInstanceName)
            {
                this.endpointSla = endpointSla;
                this.counterInstanceName = counterInstanceName;
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

            protected override Task OnStart(IMessageSession session)
            {
                counter = PerformanceCounterHelper.InstantiatePerformanceCounter("SLA violation countdown", counterInstanceName);
                timer = new Timer(RemoveOldDataPoints, null, 0, 2000);

                return Task.FromResult(0);
            }

            protected override Task OnStop(IMessageSession session)
            {
                timer.Dispose();
                counter.Dispose();

                return Task.FromResult(0);
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
                DataPoint? first = null, previous = null;

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

            PerformanceCounter counter;
            List<DataPoint> dataPoints = new List<DataPoint>();
            TimeSpan endpointSla;
            string counterInstanceName;
            // ReSharper disable once NotAccessedField.Local
            Timer timer;

            const int MaxDataPoints = 10;

            struct DataPoint
            {
                public DataPoint(TimeSpan criticalTime, DateTime occurredAt, TimeSpan processingTime)
                {
                    CriticalTime = criticalTime;
                    OccurredAt = occurredAt;
                    ProcessingTime = processingTime;
                }

                public readonly TimeSpan CriticalTime;

                public readonly DateTime OccurredAt;

                public readonly TimeSpan ProcessingTime;
            }

        }
    }
}