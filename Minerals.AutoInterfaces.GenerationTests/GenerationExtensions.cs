using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;

namespace Minerals.AutoInterfaces.GenerationTests
{
    public static class GenerationExtensions
    {
        public static GeneratorDriverRunResult RunGeneration<T>(string code)
            where T : IIncrementalGenerator, new()
        {
            var references = new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(T).Assembly.Location),
            };

            var compilation = CSharpCompilation.Create("TestAssembly")
                .AddReferences(references)
                .AddSyntaxTrees(CSharpSyntaxTree.ParseText(code))
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var driver = CSharpGeneratorDriver.Create(new T());
            return driver.RunGenerators(compilation).GetRunResult();
        }

        public static GeneratorDriverRunResult RunGeneration<T0, T1>(string code)
            where T0 : IIncrementalGenerator, new()
            where T1 : IIncrementalGenerator, new()
        {
            var references = new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(T1).Assembly.Location),
            };

            var compilation = CSharpCompilation.Create("TestAssembly")
                .AddReferences(references)
                .AddSyntaxTrees(CSharpSyntaxTree.ParseText(code))
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var driver = CSharpGeneratorDriver.Create(new T0());
            driver.RunGeneratorsAndUpdateCompilation(compilation, out var updatedCompilation, out var diagnostics);
            foreach (var diagnostic in diagnostics)
            {
                if (diagnostic.Severity is DiagnosticSeverity.Error or DiagnosticSeverity.Warning)
                {
                    throw new Exception(diagnostic.GetMessage());
                }
            }

            driver = CSharpGeneratorDriver.Create(new T1());
            return driver.RunGenerators(updatedCompilation).GetRunResult();
        }

        public static string GetGenerationOutput(GeneratorDriverRunResult result)
        {
            return string.Join(Environment.NewLine, result.Results
                .SelectMany(result => result.GeneratedSources)
                .Select(source => source.SourceText.ToString()));
        }

        public static SettingsTask ScrubVersionNumber(this SettingsTask task)
        {
            return task.ScrubLinesWithReplace(x => x.StartsWith("// Version:") ? "// Version: {Removed}" : x);
        }

        public static SettingsTask ScrubTimestamp(this SettingsTask task)
        {
            return task.ScrubLinesWithReplace(x => x.StartsWith("// Timestamp:") ? "// Timestamp: {Removed}" : x);
        }
    }
}