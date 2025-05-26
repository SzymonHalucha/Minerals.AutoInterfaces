using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;

namespace Minerals.AutoInterfaces.Benchmarks
{
    public class GeneratorsObject
    {
        private Compilation _compilation;
        private GeneratorDriver _driver;

        private GeneratorsObject(Compilation compilation, GeneratorDriver driver)
        {
            _compilation = compilation;
            _driver = driver;
        }

        public void RunGenerators()
        {
            _driver.RunGenerators(_compilation);
        }

        public void RunAndSaveGenerators()
        {
            _driver = _driver.RunGenerators(_compilation);
        }

        public void AddSourceCode(string sourceCode)
        {
            _compilation = _compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(sourceCode));
        }

        public static GeneratorsObject Create<T0, T1>(string sourceCode)
            where T0 : IIncrementalGenerator, new()
            where T1 : IIncrementalGenerator, new()
        {
            var references = new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(IIncrementalGenerator).Assembly.Location)
            };

            var compilationOptions = new CSharpCompilationOptions(
                outputKind: OutputKind.ConsoleApplication,
                optimizationLevel: OptimizationLevel.Release
            );

            var compilation = CSharpCompilation.Create("TestAssembly")
                .AddSyntaxTrees(CSharpSyntaxTree.ParseText(sourceCode))
                .AddReferences(references)
                .WithOptions(compilationOptions);

            var driver = CSharpGeneratorDriver.Create(new T0(), new T1());
            driver.RunGeneratorsAndUpdateCompilation(compilation, out var output, out var diagnostics);
            PrintCompilationDiagnostics(diagnostics);
            return new GeneratorsObject(output, driver);
        }

        private static void PrintCompilationDiagnostics(ImmutableArray<Diagnostic> diagnostics)
        {
            foreach (var diagnostic in diagnostics)
            {
                if (diagnostic.Severity is DiagnosticSeverity.Error or DiagnosticSeverity.Warning)
                {
                    Console.Error.WriteLine($"Diagnostic: {diagnostic.Id} - {diagnostic.GetMessage()}");
                }
            }
        }
    }
}