namespace NServiceBus.Metrics.PerformanceCounters
{
    using System;
    using System.IO;
    using Mono.Cecil;

    public class InnerTask
    {
        public InnerTask(string assemblyPath, string metricsAssemblyPath, string intermediateDirectory, string projectDirectory, string solutionDirectory, Action<string, string> logError)
        {
            this.metricsAssemblyPath = metricsAssemblyPath;
            this.assemblyPath = assemblyPath;
            this.intermediateDirectory = intermediateDirectory;
            this.projectDirectory = projectDirectory;
            this.solutionDirectory = solutionDirectory;
            this.logError = logError;
        }

        public void Execute()
        {
            var scriptPath = Path.Combine(intermediateDirectory, "NServiceBus.Metrics.PerformanceCounters");
            DirectoryExtensions.Delete(scriptPath);
            Directory.CreateDirectory(scriptPath);

            var assemblyModuleDefinition = ModuleDefinition.ReadModule(assemblyPath, new ReaderParameters(ReadingMode.Deferred));
            var metricsAssemblyModuleDefinition = ModuleDefinition.ReadModule(metricsAssemblyPath, new ReaderParameters(ReadingMode.Deferred));

            foreach (var variant in ScriptVariantReader.Read(assemblyModuleDefinition))
            {
                var variantPath = Path.Combine(scriptPath, variant.ToString());
                Directory.CreateDirectory(variantPath);
                CounterWriter.WriteScript(variantPath, variant, metricsAssemblyModuleDefinition, logError);
            }

            PromoteFiles(assemblyModuleDefinition, scriptPath);
        }

        void PromoteFiles(ModuleDefinition moduleDefinition, string scriptPath)
        {
            string customPath;
            if (!ScriptPromotionPathReader.TryRead(moduleDefinition, out customPath))
            {
                return;
            }
            var replicationPath = customPath
                .Replace("$(ProjectDir)", projectDirectory)
                .Replace("$(SolutionDir)", solutionDirectory);
            try
            {
                DirectoryExtensions.Delete(replicationPath);
                DirectoryExtensions.DuplicateDirectory(scriptPath, replicationPath);
            }
            catch (Exception exception)
            {
                throw new ErrorsException($"Failed to promote scripts to '{replicationPath}'. Error: {exception.Message}");
            }
        }

        string assemblyPath;
        string intermediateDirectory;
        string projectDirectory;
        string solutionDirectory;
        Action<string, string> logError;
        string metricsAssemblyPath;
    }
}