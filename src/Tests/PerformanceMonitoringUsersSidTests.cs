using NServiceBus;
using NUnit.Framework;

[TestFixture]
public class PerformanceMonitoringUsersSidTests
{
    [Test]
    public void GetUserName()
    {
        Assert.IsNotNull(PerformanceMonitoringUsersSid.Get());
    }
}