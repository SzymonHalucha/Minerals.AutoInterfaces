using BenchmarkDotNet.Attributes;

namespace Minerals.AutoInterfaces.Benchmarks
{
    public class AutoInterfaceGeneratorBenchmarks
    {
        private BenchmarkGeneration _baseline = null!;
        private BenchmarkGeneration _attributeGeneration = null!;
        private BenchmarkGeneration _interfaceGeneration = null!;

        private const string _placebo = """
        using System;

        namespace BenchmarkNamespace
        {
            public class TestClass0
            {
                public int Property1 { get; set; } = 1;
                public float Property2 { get; private set; } = 1f;
                public Action Property3 { get; private set; } = null!;
                public string Property4 { get; private set; } = "example";

                public event Action? Event1;

                private int _field0 = 1;
                private int _field1 = 1;
                private int _field2 = 1;

                public TestClass0()
                {
                    Property1 = 2;
                }

                public void Method0(int arg1)
                {
                    Console.WriteLine(arg1);
                }

                public int Method1(int arg1, int arg2)
                {
                    return arg1 + arg2;
                }

                public bool GenericMethod<T0, T1>(T0 arg1, T1 arg2) where T0 : IEquatable<T1> where T1 : IEquatable<T0>
                {
                    return arg1.Equals(arg2);
                }

                public (int A, int B) TupleMethod(int arg1, int arg2)
                {
                    return (arg1, arg2);
                }

                public void MethodWithArray(int arg1, string[] args)
                {
                    foreach (var arg in args)
                    {
                        Console.WriteLine($"{arg1}: {arg}");
                    }
                }
            }
        }
        """;

        private const string _sample = """
        using System;

        namespace BenchmarkNamespace
        {
            [Minerals.AutoInterfaces.AutoInterface]
            public class TestClass0
            {
                public int Property1 { get; set; } = 1;
                public float Property2 { get; private set; } = 1f;
                public Action Property3 { get; private set; } = null!;
                public string Property4 { get; private set; } = "example";

                public event Action? Event1;

                private int _field0 = 1;
                private int _field1 = 1;
                private int _field2 = 1;

                public TestClass0()
                {
                    Property1 = 2;
                }

                public void Method0(int arg1)
                {
                    Console.WriteLine(arg1);
                }

                public int Method1(int arg1, int arg2)
                {
                    return arg1 + arg2;
                }

                public bool GenericMethod<T0, T1>(T0 arg1, T1 arg2) where T0 : IEquatable<T1> where T1 : IEquatable<T0>
                {
                    return arg1.Equals(arg2);
                }

                public (int A, int B) TupleMethod(int arg1, int arg2)
                {
                    return (arg1, arg2);
                }

                public void MethodWithArray(int arg1, string[] args)
                {
                    foreach (var arg in args)
                    {
                        Console.WriteLine($"{arg1}: {arg}");
                    }
                }
            }
        }
        """;

        [GlobalSetup]
        public void Initialize()
        {
            _baseline = new BenchmarkGeneration();
            _baseline.SetSourceGenerators<AutoInterfaceAttributeGenerator>();
            _baseline.AddSourceCode(_placebo);
            _baseline.RunAndSaveGeneration();

            _attributeGeneration = new BenchmarkGeneration();
            _attributeGeneration.SetSourceGenerators<AutoInterfaceAttributeGenerator>();
            _attributeGeneration.AddSourceCode(_placebo);

            _interfaceGeneration = new BenchmarkGeneration();
            _interfaceGeneration.SetSourceGenerators<AutoInterfaceAttributeGenerator, AutoInterfaceGenerator>();
            _interfaceGeneration.AddSourceCode(_sample);
        }

        [Benchmark(Baseline = true)]
        public void BaselineGeneration()
        {
            _baseline.RunGeneration();
        }

        [Benchmark]
        public void AttributeOnlyGeneration()
        {
            _attributeGeneration.RunGeneration();
        }

        [Benchmark]
        public void InterfaceOnlyGeneration()
        {
            _interfaceGeneration.RunGeneration();
        }
    }
}