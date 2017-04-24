namespace NServiceBus.Metrics.PerformanceCounters
{
    using System;

    /// <summary>
    /// Configuration options that are evaluated at compile time.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class PerformanceCounterSettingsAttribute : Attribute
    {
        /// <summary>
        /// True to produce performance counter installation script in CSharp.
        /// Defaults to False.
        /// </summary>
        public bool CSharp { get; set; }

        /// <summary>
        /// True to produce performance counter installation script in Powershell.
        /// Defaults to False.
        /// </summary>
        public bool Powershell { get; set; }

        /// <summary>
        /// Path to promote performance counter installation scripts to.
        /// The token '$(SolutionDir)' will be replace with the current solution directory.
        /// The token '$(ProjectDir)' will be replace witht he current solution directory.
        /// The path calculation is performed relative to the current project directory.
        /// </summary>
        public string ScriptPromotionPath { get; set; }
    }
}