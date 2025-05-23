using BenchmarkDotNet.Attributes;
using Minerals.AutoInterfaces.Benchmarks.Utils;
using Minerals.AutoInterfaces.Attributes;

namespace Minerals.AutoInterfaces.Benchmarks
{
    public class GenerateInterfaceGeneratorBenchmarks
    {
        public BenchmarkGeneration Baseline { get; set; } = default!;
        public BenchmarkGeneration AttributeGeneration { get; set; } = default!;
        public BenchmarkGeneration InterfaceGeneration { get; set; } = default!;
        public BenchmarkGeneration BaselineDouble { get; set; } = default!;
        public BenchmarkGeneration InterfaceGenerationDouble { get; set; } = default!;

        private const string _withoutAttribute = """
        using System;

        namespace Minerals.Examples
        {
            public class TestClass
            {
                public int Property1 { get; set; } = 1;
                public int Property2 { get; } = 2;
                public int Property3 { get; set; } = 1;
                public int Property4 { get; set; } = 1;
                public int Property5 { get; set; } = 1;
                public int Property6 { get; set; } = 1;
                public int Property7 { get; set; } = 1;

                public event Action<int>? Event1;
                public event Action<int>? Event2;
                public event Action<int>? Event3;
                public event Action<int>? Event4;
                public event Action<int>? Event5;

                protected int Property3 { get; init; } = 3;

                public void Method1(int arg1)
                {
                    Console.WriteLine(arg1);
                }

                [Obsolete]
                public int Method2(int arg1, int arg2)
                {
                    return arg1 + arg2;
                }
            }
        }
        """;

        private const string _withAttribute = """
        using System;
        using Minerals.AutoInterfaces;

        namespace Minerals.Examples
        {
            [GenerateInterface]
            public class TestClass
            {
                public int Property1 { get; set; } = 1;
                public int Property2 { get; } = 2;
                public int Property3 { get; set; } = 1;
                public int Property4 { get; set; } = 1;
                public int Property5 { get; set; } = 1;
                public int Property6 { get; set; } = 1;
                public int Property7 { get; set; } = 1;

                public event Action<int>? Event1;
                public event Action<int>? Event2;
                public event Action<int>? Event3;
                public event Action<int>? Event4;
                public event Action<int>? Event5;

                protected int Property3 { get; init; } = 3;

                public void Method1(int arg1)
                {
                    Console.WriteLine(arg1);
                }

                [Obsolete]
                public int Method2(int arg1, int arg2)
                {
                    return arg1 + arg2;
                }
            }
        }
        """;

        [GlobalSetup]
        public void Initialize()
        {
            var references = BenchmarkGenerationExtensions.GetAppReferences
            (
                typeof(object),
                typeof(GenerateInterfaceAttributeGenerator)
            );
            Baseline = BenchmarkGenerationExtensions.CreateGeneration
            (
                _withoutAttribute,
                references
            );
            AttributeGeneration = BenchmarkGenerationExtensions.CreateGeneration
            (
                _withoutAttribute,
                new GenerateInterfaceAttributeGenerator(),
                references
            );
            InterfaceGeneration = BenchmarkGenerationExtensions.CreateGeneration
            (
                _withAttribute,
                new GenerateInterfaceGenerator(),
                [new GenerateInterfaceAttributeGenerator()],
                references
            );
            BaselineDouble = BenchmarkGenerationExtensions.CreateGeneration
            (
                _withoutAttribute,
                references
            );
            InterfaceGenerationDouble = BenchmarkGenerationExtensions.CreateGeneration
            (
                _withAttribute,
                new GenerateInterfaceGenerator(),
                [new GenerateInterfaceAttributeGenerator()],
                references
            );
            BaselineDouble.RunAndSaveGeneration();
            BaselineDouble.AddSourceCode("// Test Comment");
            InterfaceGenerationDouble.RunAndSaveGeneration();
            InterfaceGenerationDouble.AddSourceCode("// Test Comment");
        }

        [Benchmark(Baseline = true)]
        public void SingleGeneration_Baseline()
        {
            Baseline.RunGeneration();
        }

        [Benchmark]
        public void SingleGeneration_OnlyAttribute()
        {
            AttributeGeneration.RunGeneration();
        }

        [Benchmark]
        public void SingleGeneration_FullInterface()
        {
            InterfaceGeneration.RunGeneration();
        }

        [Benchmark]
        public void DoubleGeneration_Baseline()
        {
            BaselineDouble.RunGeneration();
        }

        [Benchmark]
        public void DoubleGeneration_FullInterface()
        {
            InterfaceGenerationDouble.RunGeneration();
        }
    }
}