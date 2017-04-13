namespace Tests
{
    using System;
    using ApprovalTests;
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

        [Test]
        public void Should_throw_for_endpoint_name_too_long()
        {
            var cache = new PerformanceCountersCache();

            var exception = Assert.Throws<Exception>(() =>
            {
                cache.Get(new CounterInstanceName("counter", new string('*', 129)));
            });
            Approvals.Verify(exception.Message);
        }

        [Test]
        public void Dispose_should_dispose_counters()
        {
            var cache = new TestablePerformanceCountersCache();

            var someCounter = cache.Get(new CounterInstanceName("RandomName", "RandomInstanceName"));
            var anotherCounter = cache.Get(new CounterInstanceName("AnotherRandomName", "AnotherRandomInstance"));

            cache.Dispose();

            Assert.IsTrue(((MockIPerformanceCounter)someCounter).Disposed);
            Assert.IsTrue(((MockIPerformanceCounter)anotherCounter).Disposed);
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