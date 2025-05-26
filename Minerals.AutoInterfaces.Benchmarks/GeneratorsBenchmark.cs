using BenchmarkDotNet.Attributes;

namespace Minerals.AutoInterfaces.Benchmarks
{
    public class GeneratorsBenchmark
    {
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

        private const string _dumpCode = """
        public class DumpClass0
        {
            public int Property1 { get; set; }

            public void Method1(int arg1, int arg2)
            {
                Console.WriteLine(arg1 + arg2);
            }
        }
        """;

        private GeneratorsObject _baselineGeneration = null!;
        private GeneratorsObject _attributeGeneration = null!;
        private GeneratorsObject _attributeSecondGeneration = null!;
        private GeneratorsObject _interfaceGeneration = null!;
        private GeneratorsObject _interfaceSecondGeneration = null!;
        private GeneratorsObject _interfaceEditedSecondGeneration = null!;

        [GlobalSetup(Target = nameof(BaselineGeneration))]
        public void InitializeBaselineGeneration()
        {
            _baselineGeneration = GeneratorsObject.Create<AutoInterfaceAttributeGenerator, AutoInterfaceGenerator>(_placebo);
            _baselineGeneration.RunAndSaveGenerators();
        }

        [GlobalSetup(Target = nameof(AttributeGeneration))]
        public void InitializeAttributeGeneration()
        {
            _attributeGeneration = GeneratorsObject.Create<AutoInterfaceAttributeGenerator, AutoInterfaceGenerator>(_placebo);
        }

        [GlobalSetup(Target = nameof(AttributeSecondGeneration))]
        public void InitializeAttributeSecondGeneration()
        {
            _attributeSecondGeneration = GeneratorsObject.Create<AutoInterfaceAttributeGenerator, AutoInterfaceGenerator>(_placebo);
            _attributeSecondGeneration.RunAndSaveGenerators();
        }

        [GlobalSetup(Target = nameof(InterfaceGeneration))]
        public void InitializeInterfaceGeneration()
        {
            _interfaceGeneration = GeneratorsObject.Create<AutoInterfaceGenerator, AutoInterfaceAttributeGenerator>(_sample);
        }

        [GlobalSetup(Target = nameof(InterfaceSecondGeneration))]
        public void InitializeInterfaceSecondGeneration()
        {
            _interfaceSecondGeneration = GeneratorsObject.Create<AutoInterfaceGenerator, AutoInterfaceAttributeGenerator>(_sample);
            _interfaceSecondGeneration.RunAndSaveGenerators();
        }

        [GlobalSetup(Target = nameof(InterfaceSecondGenerationWithDumpCode))]
        public void InitializeInterfaceSecondGenerationWithDumpCode()
        {
            _interfaceEditedSecondGeneration = GeneratorsObject.Create<AutoInterfaceGenerator, AutoInterfaceAttributeGenerator>(_sample);
            _interfaceEditedSecondGeneration.RunAndSaveGenerators();
            _interfaceEditedSecondGeneration.AddSourceCode(_dumpCode);
        }

        [Benchmark(Baseline = true)]
        public void BaselineGeneration()
        {
            _baselineGeneration.RunGenerators();
        }

        [Benchmark]
        public void AttributeGeneration()
        {
            _attributeGeneration.RunGenerators();
        }

        [Benchmark]
        public void AttributeSecondGeneration()
        {
            _attributeSecondGeneration.RunGenerators();
        }

        [Benchmark]
        public void InterfaceGeneration()
        {
            _interfaceGeneration.RunGenerators();
        }

        [Benchmark]
        public void InterfaceSecondGeneration()
        {
            _interfaceSecondGeneration.RunGenerators();
        }

        [Benchmark]
        public void InterfaceSecondGenerationWithDumpCode()
        {
            _interfaceEditedSecondGeneration.RunGenerators();
        }
    }
}