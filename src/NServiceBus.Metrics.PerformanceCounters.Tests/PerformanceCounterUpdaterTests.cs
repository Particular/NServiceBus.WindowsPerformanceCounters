namespace Tests
{
    using NServiceBus;
    using NServiceBus.Metrics.PerformanceCounters.Counters;
    using NUnit.Framework;

    [TestFixture]
    public class PerformanceCounterUpdaterTests
    {
        [Test]
        public void Meters_within_payload_should_be_converted_into_performance_counters()
        {
            var cache = new MockPerformanceCountersCache();
            var sut = new PerformanceCounterUpdater(cache);

            var payload = @"{    
    ""Context"": ""Sender@af016c07"",
    ""Meters"": [
      {
        ""Name"": ""meter 1"",
        ""Count"": 111,
        ""MeanRate"": 0.0,
        ""OneMinuteRate"": 0.0,
        ""FiveMinuteRate"": 0.0,
        ""FifteenMinuteRate"": 0.0,
        ""Unit"": ""Messages"",
        ""RateUnit"": ""s""
      },
      {
        ""Name"": ""meter 2"",
        ""Count"": 222,
        ""MeanRate"": 0.0,
        ""OneMinuteRate"": 0.0,
        ""FiveMinuteRate"": 0.0,
        ""FifteenMinuteRate"": 0.0,
        ""Unit"": ""Messages"",
        ""RateUnit"": ""s""
      },
      {
        ""Name"": ""meter 3"",
        ""Count"": 333,
        ""MeanRate"": 0.0,
        ""OneMinuteRate"": 0.0,
        ""FiveMinuteRate"": 0.0,
        ""FifteenMinuteRate"": 0.0,
        ""Unit"": ""Messages"",
        ""RateUnit"": ""s""
      }
    ]    
  }";
            sut.Update(payload);

            var performanceCounterOne = cache.Get(new CounterInstanceName("meter 1", "Sender@af016c07"));
            var performanceCounterTwo = cache.Get(new CounterInstanceName("meter 2", "Sender@af016c07"));
            var performanceCounterThree = cache.Get(new CounterInstanceName("meter 3", "Sender@af016c07"));

            Assert.AreEqual(111, performanceCounterOne.RawValue);
            Assert.AreEqual(222, performanceCounterTwo.RawValue);
            Assert.AreEqual(333, performanceCounterThree.RawValue);
        }
    }

    class MockPerformanceCountersCache : PerformanceCountersCache
    {
        protected override IPerformanceCounterInstance CreateInstance(CounterInstanceName counterInstanceName)
        {
            return new MockIPerformanceCounter();
        }
    }
}