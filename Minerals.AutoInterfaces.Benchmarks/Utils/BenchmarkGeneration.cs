namespace Minerals.AutoInterfaces.Benchmarks.Utils
{
    public class BenchmarkGeneration
    {
        public GeneratorDriver Driver { get; set; }
        public Compilation Compilation { get; set; }

        public BenchmarkGeneration(GeneratorDriver driver, Compilation compilation)
        {
            Driver = driver;
            Compilation = compilation;
        }
    }
}