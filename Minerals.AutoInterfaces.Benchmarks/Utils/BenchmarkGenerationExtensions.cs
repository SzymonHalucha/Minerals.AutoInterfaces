namespace Minerals.AutoInterfaces.Benchmarks.Utils
{
    public static class BenchmarkGenerationExtensions
    {
        public static void RunGeneration(this BenchmarkGeneration instance)
        {
            instance.Driver.RunGenerators(instance.Compilation);
        }

        public static BenchmarkGeneration CreateGeneration(string source)
        {
            return CreateGeneration(source, [], []);
        }

        public static BenchmarkGeneration CreateGeneration
        (
            string source,
            IIncrementalGenerator target
        )
        {
            return CreateGeneration(source, [target], []);
        }

        public static BenchmarkGeneration CreateGeneration
        (
            string source,
            IIncrementalGenerator target,
            IIncrementalGenerator[] additional
        )
        {
            return CreateGeneration(source, [target], additional);
        }

        public static BenchmarkGeneration CreateGeneration
        (
            string source,
            IIncrementalGenerator[] targets
        )
        {
            return CreateGeneration(source, targets, []);
        }

        public static BenchmarkGeneration CreateGeneration
        (
            string source,
            IIncrementalGenerator[] targets,
            IIncrementalGenerator[] additional
        )
        {
            var tree = CSharpSyntaxTree.ParseText(source);
            var compilation = CSharpCompilation.Create
            (
                "Minerals.Benchmarks",
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

            return new(CSharpGeneratorDriver.Create(targets), newCompilation);
        }
    }
}