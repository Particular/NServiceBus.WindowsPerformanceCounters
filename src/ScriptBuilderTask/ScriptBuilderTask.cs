namespace NServiceBus.Metrics.PerformanceCounters
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    public class ScriptBuilderTask : Task
    {
        [Required]
        public string References { get; set; }

        [Required]
        public ITaskItem[] ReferenceCopyLocalPaths { get; set; }

        [Required]
        public string IntermediateDirectory { get; set; }

        [Required]
        public string ProjectDirectory { get; set; }

        [Required]
        public string SolutionDirectory { get; set; }

        public Dictionary<string, string> ReferenceDictionary { get; } = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        public override bool Execute()
        {
            logger = new BuildLogger(BuildEngine);
            logger.LogInfo($"ScriptBuilderTask (version {typeof(ScriptBuilderTask).Assembly.GetName().Version}) Executing");

            var stopwatch = Stopwatch.StartNew();

            var referenceCopyLocalPaths = ReferenceCopyLocalPaths.Select(x => x.ItemSpec);
            var splitReferences = References.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Union(referenceCopyLocalPaths, StringComparer.InvariantCultureIgnoreCase);
            foreach (var filePath in splitReferences)
            {
                var fileNameWithExtension = Path.GetFileName(filePath);
                ReferenceDictionary[fileNameWithExtension] = filePath;

                logger.LogInfo($"ScriptBuilderTask ({fileNameWithExtension} {filePath})");
            }

            try
            {
                ValidateInputs();
                Action<string, string> logError = (error, file) => { logger.LogError(error, file); };
                var innerTask = new InnerTask(ReferenceDictionary["NServiceBus.Metrics.dll"], IntermediateDirectory, ProjectDirectory, SolutionDirectory, logError);
                innerTask.Execute();
            }
            catch (ErrorsException exception)
            {
                logger.LogError(exception.Message, exception.FileName);
            }
            catch (Exception exception)
            {
                logger.LogError(exception.ToFriendlyString());
            }
            finally
            {
                logger.LogInfo($"  Finished ScriptBuilderTask {stopwatch.ElapsedMilliseconds}ms.");
            }
            return !logger.ErrorOccurred;
        }


        void ValidateInputs()
        {
            if (!Directory.Exists(IntermediateDirectory))
            {
                throw new ErrorsException($"IntermediateDirectory '{IntermediateDirectory}' does not exist.");
            }

            if (!Directory.Exists(ProjectDirectory))
            {
                throw new ErrorsException($"ProjectDirectory '{ProjectDirectory}' does not exist.");
            }

            if (!Directory.Exists(SolutionDirectory))
            {
                throw new ErrorsException($"SolutionDirectory '{SolutionDirectory}' does not exist.");
            }

            string filePath;
            if (!ReferenceDictionary.TryGetValue("NServiceBus.Metrics.dll", out filePath))
            {
                throw new ErrorsException("NServiceBus.Metrics.dll is not referenced in this assembly.");
            }
        }

        BuildLogger logger;
    }
}