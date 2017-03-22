using System;
using ApprovalTests;
using NUnit.Framework;

[TestFixture]
public class PerformanceCounterHelperTests
{
    [Test]
    public void Should_throw_for_endpoint_name_too_long()
    {
        var exception = Assert.Throws<Exception>(() =>
        {
            PerformanceCounterHelper.InstantiatePerformanceCounter("counter", new string('*', 129));
        });
        Approvals.Verify(exception.Message);
    }
}