namespace Minerals.AutoInterfaces.Benchmarks.Utils
{
    public readonly struct BenchmarkGeneration(GeneratorDriver driver, Compilation compilation)
    {
        public readonly GeneratorDriver Driver { get; } = driver;
        public readonly Compilation Compilation { get; } = compilation;
    }
}