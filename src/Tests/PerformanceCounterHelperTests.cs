using System;
using System.Diagnostics;
using ApprovalTests;
using NServiceBus;
using NUnit.Framework;

[TestFixture]
public class PerformanceCounterHelperTests
{
    [Test]
    public void Should_throw_for_endpoint_name_too_long()
    {
        PerformanceCounter counter;
        var exception = Assert.Throws<Exception>(() =>
        {
            PerformanceCounterHelper.TryToInstantiatePerformanceCounter("counter", new string('*', 129), out counter, false);
        });
        Approvals.Verify(exception.Message);
    }
}