namespace Tests
{
    using System.Collections.Generic;
    using NServiceBus;
    using NUnit.Framework;

    [TestFixture]
    public class PerformanceCounterUpdaterTests
    {
        [Test]
        public void Meters_within_payload_should_be_converted_into_performance_counters()
        {
            var cache = new MockPerformanceCountersCache();
            var sut = new PerformanceCounterUpdater(cache, new Dictionary<string, CounterInstanceName?>());

            sut.Update(PayloadWithRandomMeters);

            var performanceCounterOne = cache.Get(new CounterInstanceName("meter 1", "Sender@af016c07"));
            var performanceCounterTwo = cache.Get(new CounterInstanceName("meter 2", "Sender@af016c07"));
            var performanceCounterThree = cache.Get(new CounterInstanceName("meter 3", "Sender@af016c07"));

            Assert.AreEqual(111, performanceCounterOne.RawValue);
            Assert.AreEqual(222, performanceCounterTwo.RawValue);
            Assert.AreEqual(333, performanceCounterThree.RawValue);
        }

        const string PayloadWithRandomMeters = @"{    
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

        [Test]
        public void Meters_that_need_mapping_within_payload_should_be_converted_into_performance_counters()
        {
            var cache = new MockPerformanceCountersCache();

            var legacyInstanceNameMap = new Dictionary<string, CounterInstanceName?>
            {
                { "# of message failures / sec", new CounterInstanceName("# of msgs failures / sec", "queueAddress") },
                { "# of messages pulled from the input queue / sec", new CounterInstanceName("# of msgs pulled from the input queue /sec", "queueAddress") },
                { "# of messages successfully processed / sec", new CounterInstanceName("# of msgs successfully processed / sec", "queueAddress") },
            };

            var sut = new PerformanceCounterUpdater(cache, legacyInstanceNameMap);

            sut.Update(PayloadWithNewMeterNamesThatNeedToBeMappedToLegacyMeterNames);

            var performanceCounterOne = cache.Get(new CounterInstanceName("# of msgs failures / sec", "queueAddress"));
            var performanceCounterTwo = cache.Get(new CounterInstanceName("# of msgs pulled from the input queue /sec", "queueAddress"));
            var performanceCounterThree = cache.Get(new CounterInstanceName("# of msgs successfully processed / sec", "queueAddress"));

            Assert.AreEqual(111, performanceCounterOne.RawValue);
            Assert.AreEqual(222, performanceCounterTwo.RawValue);
            Assert.AreEqual(333, performanceCounterThree.RawValue);
        }

        const string PayloadWithNewMeterNamesThatNeedToBeMappedToLegacyMeterNames = @"{    
    ""Context"": ""Sender@af016c07"",
    ""Meters"": [
      {
        ""Name"": ""# of message failures / sec"",
        ""Count"": 111,
        ""MeanRate"": 0.0,
        ""OneMinuteRate"": 0.0,
        ""FiveMinuteRate"": 0.0,
        ""FifteenMinuteRate"": 0.0,
        ""Unit"": ""Messages"",
        ""RateUnit"": ""s""
      },
      {
        ""Name"": ""# of messages pulled from the input queue / sec"",
        ""Count"": 222,
        ""MeanRate"": 0.0,
        ""OneMinuteRate"": 0.0,
        ""FiveMinuteRate"": 0.0,
        ""FifteenMinuteRate"": 0.0,
        ""Unit"": ""Messages"",
        ""RateUnit"": ""s""
      },
      {
        ""Name"": ""# of messages successfully processed / sec"",
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

        [Test]
        public void Timers_within_payload_should_be_converted_into_performance_counters()
        {
            var cache = new MockPerformanceCountersCache();
            var sut = new PerformanceCounterUpdater(cache, new Dictionary<string, CounterInstanceName?>());

            sut.Update(PayloadWithRandomTimers);

            var performanceCounterOne = cache.Get(new CounterInstanceName("Critical Time", "Sender@af016c07"));
            var performanceCounterTwo = cache.Get(new CounterInstanceName("Processing Time", "Sender@af016c07"));

            Assert.AreEqual(11, performanceCounterOne.RawValue);
            Assert.AreEqual(22, performanceCounterTwo.RawValue);
        }

        const string PayloadWithRandomTimers = @"{	
    ""Context"": ""Sender@af016c07"",    
    ""Timers"": [
      {
        ""Name"": ""Critical Time"",
        ""Count"": 0,
        ""ActiveSessions"": 0,
        ""TotalTime"": 11,
        ""Rate"": {
          ""MeanRate"": 0.0,
          ""OneMinuteRate"": 0.0,
          ""FiveMinuteRate"": 0.0,
          ""FifteenMinuteRate"": 0.0
        },
        ""Histogram"": {
          ""LastValue"": 0.0,
          ""Min"": 0.0,
          ""Mean"": 0.0,
          ""StdDev"": 0.0,
          ""Median"": 0.0,
          ""Percentile75"": 0.0,
          ""Percentile95"": 0.0,
          ""Percentile98"": 0.0,
          ""Percentile99"": 0.0,
          ""Percentile999"": 0.0,
          ""SampleSize"": 0
        },
        ""Unit"": ""Messages"",
        ""RateUnit"": ""s"",
        ""DurationUnit"": ""ms""
      },
      {
        ""Name"": ""Processing Time"",
        ""Count"": 0,
        ""ActiveSessions"": 0,
        ""TotalTime"": 22,
        ""Rate"": {
          ""MeanRate"": 0.0,
          ""OneMinuteRate"": 0.0,
          ""FiveMinuteRate"": 0.0,
          ""FifteenMinuteRate"": 0.0
        },
        ""Histogram"": {
          ""LastValue"": 0.0,
          ""Min"": 0.0,
          ""Mean"": 0.0,
          ""StdDev"": 0.0,
          ""Median"": 0.0,
          ""Percentile75"": 0.0,
          ""Percentile95"": 0.0,
          ""Percentile98"": 0.0,
          ""Percentile99"": 0.0,
          ""Percentile999"": 0.0,
          ""SampleSize"": 0
        },
        ""Unit"": ""Messages"",
        ""RateUnit"": ""s"",
        ""DurationUnit"": ""ms""
      }
    ]
  }";
    }

    class MockPerformanceCountersCache : PerformanceCountersCache
    {
        protected override IPerformanceCounterInstance CreateInstance(CounterInstanceName counterInstanceName)
        {
            return new MockIPerformanceCounter();
        }
    }
}