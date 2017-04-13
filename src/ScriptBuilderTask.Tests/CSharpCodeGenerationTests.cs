namespace ScriptBuilderTask.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using ApprovalTests;
    using NServiceBus.Metrics.PerformanceCounters;
    using NUnit.Framework;

    [TestFixture]
    public class CSharpCodeGenerationTests
    {
        InnerTask task;
        string tempPath;

        [SetUp]
        public void SetUp()
        {
            var testDirectory = TestContext.CurrentContext.TestDirectory;
            tempPath = testDirectory;
            var assemblyPath = Path.Combine(testDirectory, "ScriptBuilderTask.Tests.dll");
            var metricsAssemblyPath = Path.Combine(testDirectory, "NServiceBus.Metrics.dll");
            var intermediatePath = Path.Combine(tempPath, "IntermediatePath");
            var promotePath = Path.Combine(tempPath, "PromotePath");
            Directory.CreateDirectory(tempPath);
            Directory.CreateDirectory(intermediatePath);

            Action<string, string> logError = (error, s1) => { throw new Exception(error); };
            task = new InnerTask(assemblyPath, metricsAssemblyPath, intermediatePath, "TheProjectDir", promotePath, logError);
        }

        [Test]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Generates()
        {
            task.Execute();

            var cSharpCode = Directory.EnumerateFiles(tempPath, "*.g.cs", SearchOption.AllDirectories).Single();
            Approvals.VerifyFile(cSharpCode);
        }
    }
}