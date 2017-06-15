namespace NServiceBus.Metrics.PerformanceCounters
{
    using Microsoft.Build.Framework;

    class BuildLogger
    {
        public BuildLogger(IBuildEngine buildEngine)
        {
            this.buildEngine = buildEngine;
        }

        public void LogInfo(string message)
        {
            buildEngine.LogMessageEvent(new BuildMessageEventArgs(PrependMessage(message), "", "PerformanceCounterScriptTask", MessageImportance.Normal));
        }

        static string PrependMessage(string message)
        {
            return $"PerformanceCounterScriptTask: {message}";
        }

        public void LogError(string message, string file = null)
        {
            ErrorOccurred = true;
            buildEngine.LogErrorEvent(new BuildErrorEventArgs("", "", file, 0, 0, 0, 0, PrependMessage(message), "", "PerformanceCounterScriptTask"));
        }

        public bool ErrorOccurred;

        IBuildEngine buildEngine;
    }
}