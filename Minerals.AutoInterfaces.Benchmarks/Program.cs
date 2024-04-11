namespace Minerals.AutoInterfaces.Benchmarks
{
    public class Program
    {
        public static void Main()
        {
            BenchmarkRunner.Run<GenerateInterfaceGeneratorBenchmarks>
            (
                DefaultConfig.Instance
                    .WithOrderer(new DefaultOrderer(SummaryOrderPolicy.FastestToSlowest))
                    // .AddJob(Job.Default.WithRuntime(ClrRuntime.Net481))
                    // .AddJob(Job.Default.WithRuntime(CoreRuntime.Core60))
                    .AddJob(Job.Default.WithRuntime(CoreRuntime.Core80))
                    .AddValidator(JitOptimizationsValidator.FailOnError)
                    .AddDiagnoser(MemoryDiagnoser.Default)
            );
        }
    }
}