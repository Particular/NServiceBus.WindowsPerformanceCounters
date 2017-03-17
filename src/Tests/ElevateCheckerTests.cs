using NUnit.Framework;

[TestFixture]
public class ElevateCheckerTests
{
    [Test]
    public void Assert_does_no_throw()
    {
        // Cant assert the output since we dont know who is running the test
        ElevateChecker.IsCurrentUserElevated();
    }
}