namespace Tests
{
    using NServiceBus;
    using NUnit.Framework;

    [TestFixture]
    public class PerformanceCountersCacheTests
    {
        [Test]
        public void Get_the_same_counter_twice_returns_the_same()
        {
            var cache = new TestablePerformanceCountersCache();

            var counterName = "CounterName";
            var instanceName = "InstancenName";

            var firstCounter = cache.Get(new CounterInstanceName(counterName, instanceName));
            var secondCounter = cache.Get(new CounterInstanceName(counterName, instanceName));

            Assert.NotNull(firstCounter);
            Assert.NotNull(secondCounter);
            Assert.AreSame(firstCounter, secondCounter);
            Assert.AreEqual(1, cache.CountersCreated);
        }

        class TestablePerformanceCountersCache : PerformanceCountersCache
        {
            public int CountersCreated { get; private set; }

            protected override IPerformanceCounterInstance CreateInstance(CounterInstanceName counterInstanceName)
            {
                CountersCreated++;
                return new MockIPerformanceCounter();
            }
        }
    }
}