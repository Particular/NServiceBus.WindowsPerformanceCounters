// ReSharper disable NotAccessedField.Local
namespace NServiceBus.Metrics.PerformanceCounters
{
    using System;
    using System.IO;
    using Mono.Cecil;

    public class InnerTask
    {
        string assemblyPath;
        string intermediateDirectory;
        string projectDirectory;
        string solutionDirectory;
        Action<string, string> logError;

        public InnerTask(string assemblyPath, string intermediateDirectory, string projectDirectory, string solutionDirectory, Action<string, string> logError)
        {
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

            var moduleDefinition = ModuleDefinition.ReadModule(assemblyPath, new ReaderParameters(ReadingMode.Deferred));

            CounterWriter.WriteScript(scriptPath, moduleDefinition, logError);
        }
    }

    static class DirectoryExtensions
    {
        public static void Delete(string path)
        {
            if (!Directory.Exists(path))
            {
                return;
            }
            Directory.Delete(path, true);
        }
        public static void DuplicateDirectory(string source, string destination)
        {
            foreach (var dirPath in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(source, destination));
            }

            foreach (var newPath in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(source, destination), true);
            }
        }

    }
}