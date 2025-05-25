using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;

namespace Minerals.AutoInterfaces.Benchmarks
{
    public class BenchmarkGeneration
    {
        private GeneratorDriver _driver;
        private Compilation _compilation;

        public BenchmarkGeneration()
        {
            _driver = default!;
            _compilation = CSharpCompilation.Create("BenchmarkTestAssembly")
                .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(typeof(AutoInterfaceGenerator).Assembly.Location))
                .WithOptions(new CSharpCompilationOptions(outputKind: OutputKind.ConsoleApplication, optimizationLevel: OptimizationLevel.Release));
        }

        public void RunAndSaveGeneration()
        {
            _driver.RunGeneratorsAndUpdateCompilation(_compilation, out var outputCompilation, out var diagnostics);
            _compilation = outputCompilation;
            foreach (var item in diagnostics)
            {
                if (item.Severity is DiagnosticSeverity.Error or DiagnosticSeverity.Warning)
                {
                    throw new Exception(item.GetMessage());
                }
            }
        }

        public void RunGeneration()
        {
            _driver.RunGenerators(_compilation);
        }

        public void AddSourceCode(string sourceCode)
        {
            var tree = CSharpSyntaxTree.ParseText(sourceCode);
            _compilation = _compilation.AddSyntaxTrees(tree);
        }

        public void SetSourceGenerators<T>() where T : IIncrementalGenerator, new()
        {
            _driver = CSharpGeneratorDriver.Create(new T());
        }

        public void SetSourceGenerators<T0, T1>() where T0 : IIncrementalGenerator, new() where T1 : IIncrementalGenerator, new()
        {
            _driver = CSharpGeneratorDriver.Create(new T0(), new T1());
        }
    }
}