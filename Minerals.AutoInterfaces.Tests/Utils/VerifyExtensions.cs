namespace Minerals.AutoInterfaces.Tests.Utils
{
    public static class VerifyExtensions
    {
        public static bool ScrubSingleLineComments { get; set; } = true;
        public static bool RemoveEmptyLines { get; set; } = false;

        private static bool _isInitialized = false;

        public static void InitializeGlobalSettings()
        {
            if (_isInitialized)
            {
                return;
            }
            DiffTools.UseOrder(DiffTool.VisualStudioCode, DiffTool.VisualStudio, DiffTool.Rider);
            VerifyBase.UseProjectRelativeDirectory("Snapshots");
            VerifierSettings.UseEncoding(Encoding.UTF8);
            VerifySourceGenerators.Initialize();
            _isInitialized = true;
        }

        public static Task VerifyIncrementalGenerators(this VerifyBase instance, IIncrementalGenerator target)
        {
            return VerifyIncrementalGenerators(instance, [target], []);
        }

        public static Task VerifyIncrementalGenerators
        (
            this VerifyBase instance,
            IIncrementalGenerator target,
            IIncrementalGenerator[] additional
        )
        {
            return VerifyIncrementalGenerators(instance, [target], additional);
        }

        public static Task VerifyIncrementalGenerators
        (
            this VerifyBase instance,
            IIncrementalGenerator[] targets,
            IIncrementalGenerator[] additional
        )
        {
            var compilation = CSharpCompilation.Create("Minerals.Tests");
            CSharpGeneratorDriver.Create(additional)
                .RunGeneratorsAndUpdateCompilation
                (
                    compilation,
                    out var newCompilation,
                    out _
                );

            var driver = CSharpGeneratorDriver.Create(targets)
                .RunGenerators(newCompilation);

            return ApplyDynamicSettings(instance.Verify(driver.GetRunResult()));
        }

        public static Task VerifyIncrementalGenerators(this VerifyBase instance, string source, IIncrementalGenerator target)
        {
            return VerifyIncrementalGenerators(instance, source, [target], []);
        }

        public static Task VerifyIncrementalGenerators
        (
            this VerifyBase instance,
            string source,
            IIncrementalGenerator target,
            IIncrementalGenerator[] additional
        )
        {
            return VerifyIncrementalGenerators(instance, source, [target], additional);
        }

        public static Task VerifyIncrementalGenerators
        (
            this VerifyBase instance,
            string source,
            IIncrementalGenerator[] targets,
            IIncrementalGenerator[] additional
        )
        {
            var tree = CSharpSyntaxTree.ParseText(source);
            var compilation = CSharpCompilation.Create
            (
                "Minerals.Tests",
                [tree],
                [MetadataReference.CreateFromFile(tree.GetType().Assembly.Location)]
            );

            CSharpGeneratorDriver.Create(additional)
                .RunGeneratorsAndUpdateCompilation
                (
                    compilation,
                    out var newCompilation,
                    out _
                );

            var driver = CSharpGeneratorDriver.Create(targets)
                .RunGenerators(newCompilation);

            return ApplyDynamicSettings(instance.Verify(driver.GetRunResult()));
        }

        private static SettingsTask ApplyDynamicSettings(SettingsTask task)
        {
            if (ScrubSingleLineComments)
            {
                task.ScrubLines("cs", x =>
                {
                    return x.StartsWith("//");
                });
            }
            if (RemoveEmptyLines)
            {
                task.ScrubEmptyLines();
            }
            return task;
        }
    }
}