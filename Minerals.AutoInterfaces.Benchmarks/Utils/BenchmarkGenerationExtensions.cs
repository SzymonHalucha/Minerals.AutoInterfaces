namespace Minerals.AutoInterfaces.Benchmarks.Utils
{
    public static class BenchmarkGenerationExtensions
    {
        public static IEnumerable<MetadataReference> GetAppReferences(params Type[] additionalReferences)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetReferencedAssemblies());
            assemblies = assemblies.Concat(additionalReferences.Select(x => x.Assembly.GetName()));
            return assemblies.Select(x => MetadataReference.CreateFromFile(Assembly.Load(x).Location));
        }

        public static void RunAndUpdateGeneration(this BenchmarkGeneration instance)
        {
            instance.Driver.RunGeneratorsAndUpdateCompilation
            (
                instance.Compilation,
                out var cmp,
                out _
            );
            instance.Compilation = cmp;
        }

        public static void RunGeneration(this BenchmarkGeneration instance)
        {
            instance.Driver.RunGenerators(instance.Compilation);
        }

        public static BenchmarkGeneration CreateGeneration(string source, IEnumerable<MetadataReference> references)
        {
            var targets = Array.Empty<IIncrementalGenerator>();
            var additional = Array.Empty<IIncrementalGenerator>();
            return CreateGeneration(source, targets, additional, references);
        }

        public static BenchmarkGeneration CreateGeneration
        (
            string source,
            IIncrementalGenerator target,
            IEnumerable<MetadataReference> references
        )
        {
            var targets = new IIncrementalGenerator[] { target };
            var additional = Array.Empty<IIncrementalGenerator>();
            return CreateGeneration(source, targets, additional, references);
        }

        public static BenchmarkGeneration CreateGeneration
        (
            string source,
            IIncrementalGenerator target,
            IEnumerable<IIncrementalGenerator> additional,
            IEnumerable<MetadataReference> references
        )
        {
            var targets = new IIncrementalGenerator[] { target };
            return CreateGeneration(source, targets, additional, references);
        }

        public static BenchmarkGeneration CreateGeneration
        (
            string source,
            IEnumerable<IIncrementalGenerator> targets,
            IEnumerable<MetadataReference> references
        )
        {
            var additional = Array.Empty<IIncrementalGenerator>();
            return CreateGeneration(source, targets, additional, references);
        }

        public static BenchmarkGeneration CreateGeneration
        (
            string source,
            IEnumerable<IIncrementalGenerator> targets,
            IEnumerable<IIncrementalGenerator> additional,
            IEnumerable<MetadataReference> references
        )
        {
            var tree = CSharpSyntaxTree.ParseText(source);
            var cSharpCmp = CSharpCompilation.Create("Benchmarks")
                .AddReferences(MetadataReference.CreateFromFile(tree.GetType().Assembly.Location))
                .AddReferences(references)
                .AddSyntaxTrees(tree)
                .WithOptions(new CSharpCompilationOptions
                (
                    outputKind: OutputKind.ConsoleApplication,
                    nullableContextOptions: NullableContextOptions.Enable,
                    optimizationLevel: OptimizationLevel.Release
                ));

            CSharpGeneratorDriver.Create(additional.ToArray())
                .RunGeneratorsAndUpdateCompilation
                (
                    cSharpCmp,
                    out var cmp,
                    out _
                );

            return new BenchmarkGeneration(CSharpGeneratorDriver.Create(targets.ToArray()), cmp);
        }
    }
}