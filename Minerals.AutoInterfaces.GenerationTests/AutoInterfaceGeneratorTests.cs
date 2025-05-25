namespace Minerals.AutoInterfaces.GenerationTests
{
    [UsesVerify, TestClass]
    public partial class AutoInterfaceGeneratorTests
    {
        [TestMethod]
        public Task WithoutNamespace()
        {
            const string source = """
            [Minerals.AutoInterfaces.AutoInterface]
            public class TestClass { }
            """;
            var output = GenerationExtensions.RunGeneration<AutoInterfaceAttributeGenerator, AutoInterfaceGenerator>(source);
            return Verify(GenerationExtensions.GetGenerationOutput(output), "cs")
                .UseDirectory("Snapshots")
                .ScrubVersionNumber()
                .ScrubTimestamp();
        }

        [TestMethod]
        public Task WithNamespace()
        {
            const string source = """
            namespace TestNamespace
            {
                [Minerals.AutoInterfaces.AutoInterface]
                public class TestClass { }
            }
            """;
            var output = GenerationExtensions.RunGeneration<AutoInterfaceAttributeGenerator, AutoInterfaceGenerator>(source);
            return Verify(GenerationExtensions.GetGenerationOutput(output), "cs")
                .UseDirectory("Snapshots")
                .ScrubVersionNumber()
                .ScrubTimestamp();
        }

        [TestMethod]
        public Task WithSubNamespace()
        {
            const string source = """
            namespace TestNamespace.SubNamespace
            {
                [Minerals.AutoInterfaces.AutoInterface]
                public class TestClass { }
            }
            """;
            var output = GenerationExtensions.RunGeneration<AutoInterfaceAttributeGenerator, AutoInterfaceGenerator>(source);
            return Verify(GenerationExtensions.GetGenerationOutput(output), "cs")
                .UseDirectory("Snapshots")
                .ScrubVersionNumber()
                .ScrubTimestamp();
        }

        [TestMethod]
        public Task WithInterfaceCustomName()
        {
            const string source = """
            namespace TestNamespace.SubNamespace
            {
                [Minerals.AutoInterfaces.AutoInterface("IExampleInterface")]
                public class TestClass { }
            }
            """;
            var output = GenerationExtensions.RunGeneration<AutoInterfaceAttributeGenerator, AutoInterfaceGenerator>(source);
            return Verify(GenerationExtensions.GetGenerationOutput(output), "cs")
                .UseDirectory("Snapshots")
                .ScrubVersionNumber()
                .ScrubTimestamp();
        }

        [TestMethod]
        public Task ShouldGenerateOnlyWithPublicProperty()
        {
            const string source = """
            namespace TestNamespace
            {
                [Minerals.AutoInterfaces.AutoInterface]
                public class TestClass
                {
                    public int Property1 { get; set; }
                    protected int Property2 { get; set; }
                    internal int Property3 { get; set; }
                    private int Property4 { get; set; }
                }
            }
            """;
            var output = GenerationExtensions.RunGeneration<AutoInterfaceAttributeGenerator, AutoInterfaceGenerator>(source);
            return Verify(GenerationExtensions.GetGenerationOutput(output), "cs")
                .UseDirectory("Snapshots")
                .ScrubVersionNumber()
                .ScrubTimestamp();
        }

        [TestMethod]
        public Task ShouldGenerateWithoutAttributes()
        {
            const string source = """
            namespace TestNamespace
            {
                [Minerals.AutoInterfaces.AutoInterface]
                public class TestClass
                {
                    [System.Obsolete]
                    public int Property1 { get; set; }
                    [System.Obsolete] public int Property2 { get; set; }
                    public float Property3 { get; [System.Obsolete] set; }
                }
            }
            """;
            var output = GenerationExtensions.RunGeneration<AutoInterfaceAttributeGenerator, AutoInterfaceGenerator>(source);
            return Verify(GenerationExtensions.GetGenerationOutput(output), "cs")
                .UseDirectory("Snapshots")
                .ScrubVersionNumber()
                .ScrubTimestamp();
        }

        [TestMethod]
        public Task ShouldGenerateWithoutAssignments()
        {
            const string source = """
            namespace TestNamespace
            {
                [Minerals.AutoInterfaces.AutoInterface]
                public class TestClass
                {
                    public int Property1 { get; set; } = 1;
                    public int Property2 { get; } = 2;
                    public int Property3 { get; init; } = 3;
                    public float Property4 { get; set; } = 4.5f;
                }
            }
            """;
            var output = GenerationExtensions.RunGeneration<AutoInterfaceAttributeGenerator, AutoInterfaceGenerator>(source);
            return Verify(GenerationExtensions.GetGenerationOutput(output), "cs")
                .UseDirectory("Snapshots")
                .ScrubVersionNumber()
                .ScrubTimestamp();
        }

        [TestMethod]
        public Task ShouldGenerateOnlyPublicAccessors()
        {
            const string source = """
            namespace TestNamespace
            {
                [Minerals.AutoInterfaces.AutoInterface]
                public class TestClass
                {
                    public int Property1 { get; set; }
                    public int Property2 { get; }
                    public int Property3 { private get; set; }
                    public int Property4 { get; protected set; }
                    public string Property5 { get; init; }
                    public float Property6 { get; internal init; }
                }
            }
            """;
            var output = GenerationExtensions.RunGeneration<AutoInterfaceAttributeGenerator, AutoInterfaceGenerator>(source);
            return Verify(GenerationExtensions.GetGenerationOutput(output), "cs")
                .UseDirectory("Snapshots")
                .ScrubVersionNumber()
                .ScrubTimestamp();
        }

        [TestMethod]
        public Task ShouldGenerateWithoutPropertiesImplementations()
        {
            const string source = """
            namespace TestNamespace
            {
                [Minerals.AutoInterfaces.AutoInterface]
                public class TestClass
                {
                    public int Property1 => _field;
                    public int Property2 { get => _field; }
                    public int Property3 { get { return _field; } set { _field = value; } }
                    public int Property4
                    {
                        get
                        {
                            return _field;
                        }
                        set
                        {
                            _field = value;
                        }
                    }

                    private int _field;
                }
            }
            """;
            var output = GenerationExtensions.RunGeneration<AutoInterfaceAttributeGenerator, AutoInterfaceGenerator>(source);
            return Verify(GenerationExtensions.GetGenerationOutput(output), "cs")
                .UseDirectory("Snapshots")
                .ScrubVersionNumber()
                .ScrubTimestamp();
        }

        [TestMethod]
        public Task ShouldNotGenerateFields()
        {
            const string source = """
            namespace TestNamespace
            {
                [Minerals.AutoInterfaces.AutoInterface]
                public class TestClass
                {
                    public int Field1 = 1;
                    protected float Field2 = 2f;
                    internal int Field3 = 3;
                    private int Field4;
                }
            }
            """;
            var output = GenerationExtensions.RunGeneration<AutoInterfaceAttributeGenerator, AutoInterfaceGenerator>(source);
            return Verify(GenerationExtensions.GetGenerationOutput(output), "cs")
                .UseDirectory("Snapshots")
                .ScrubVersionNumber()
                .ScrubTimestamp();
        }

        [TestMethod]
        public Task ShouldGenerateEventFields()
        {
            const string source = """
            namespace TestNamespace
            {
                [Minerals.AutoInterfaces.AutoInterface]
                public class TestClass
                {
                    public event System.EventHandler? Event1;
                    public event System.Action<int>? Event2;
                }
            }
            """;
            var output = GenerationExtensions.RunGeneration<AutoInterfaceAttributeGenerator, AutoInterfaceGenerator>(source);
            return Verify(GenerationExtensions.GetGenerationOutput(output), "cs")
                .UseDirectory("Snapshots")
                .ScrubVersionNumber()
                .ScrubTimestamp();
        }

        [TestMethod]
        public Task ShouldGenerateMethods()
        {
            const string source = """
            namespace TestNamespace
            {
                [Minerals.AutoInterfaces.AutoInterface]
                public class TestClass
                {
                    public void Method1() { }
                    public int Method2(int arg) => arg;
                    public float Method3(float arg1, float arg2) => arg1 + arg2;
                }
            }
            """;
            var output = GenerationExtensions.RunGeneration<AutoInterfaceAttributeGenerator, AutoInterfaceGenerator>(source);
            return Verify(GenerationExtensions.GetGenerationOutput(output), "cs")
                .UseDirectory("Snapshots")
                .ScrubVersionNumber()
                .ScrubTimestamp();
        }

        [TestMethod]
        public Task ShouldGenerateMethodsWithoutImplementation()
        {
            const string source = """
            namespace TestNamespace
            {
                [Minerals.AutoInterfaces.AutoInterface]
                public class TestClass
                {
                    public void Method1()
                    {
                        Console.WriteLine("Hello, World!");
                    }
                }
            }
            """;
            var output = GenerationExtensions.RunGeneration<AutoInterfaceAttributeGenerator, AutoInterfaceGenerator>(source);
            return Verify(GenerationExtensions.GetGenerationOutput(output), "cs")
                .UseDirectory("Snapshots")
                .ScrubVersionNumber()
                .ScrubTimestamp();
        }

        [TestMethod]
        public Task ShouldGenerateMethodsWithoutAttributes()
        {
            const string source = """
            namespace TestNamespace
            {
                [Minerals.AutoInterfaces.AutoInterface]
                public class TestClass
                {
                    [System.Obsolete]
                    public void Method1() { }
                    [System.Obsolete] public void Method2(int arg) { }
                }
            }
            """;
            var output = GenerationExtensions.RunGeneration<AutoInterfaceAttributeGenerator, AutoInterfaceGenerator>(source);
            return Verify(GenerationExtensions.GetGenerationOutput(output), "cs")
                .UseDirectory("Snapshots")
                .ScrubVersionNumber()
                .ScrubTimestamp();
        }

        [TestMethod]
        public Task ShouldGenerateOnlyPublicMethods()
        {
            const string source = """
            namespace TestNamespace
            {
                [Minerals.AutoInterfaces.AutoInterface]
                public class TestClass
                {
                    public void Method1() { }
                    protected void Method2(int arg) { }
                    internal void Method3() { }
                    private void Method4(int arg) { }
                }
            }
            """;
            var output = GenerationExtensions.RunGeneration<AutoInterfaceAttributeGenerator, AutoInterfaceGenerator>(source);
            return Verify(GenerationExtensions.GetGenerationOutput(output), "cs")
                .UseDirectory("Snapshots")
                .ScrubVersionNumber()
                .ScrubTimestamp();
        }

        [TestMethod]
        public Task ShouldGenerateWithoutStaticMembers()
        {
            const string source = """
            namespace TestNamespace
            {
                [Minerals.AutoInterfaces.AutoInterface]
                public class TestClass
                {
                    public static int Property1 { get; set; } = 1;
                    public int Property2 { get; set; } = 2;

                    public static event System.Action<int>? Event1;
                    public static int Field1 = 1;

                    public static void Method1() { }
                    public void Method2() { }
                }
            }
            """;
            var output = GenerationExtensions.RunGeneration<AutoInterfaceAttributeGenerator, AutoInterfaceGenerator>(source);
            return Verify(GenerationExtensions.GetGenerationOutput(output), "cs")
                .UseDirectory("Snapshots")
                .ScrubVersionNumber()
                .ScrubTimestamp();
        }

        [TestMethod]
        public Task ShouldGenerateWithoutGlobalUsings()
        {
            const string source = """
            global using System.Collections.Generic;
            global using System.Linq;

            namespace TestNamespace
            {
                [Minerals.AutoInterfaces.AutoInterface]
                public class TestClass
                {
                    public void Method1() { }
                    protected void Method2(int arg) { }
                    internal void Method3() { }
                    private void Method4(int arg) { }
                }
            }
            """;
            var output = GenerationExtensions.RunGeneration<AutoInterfaceAttributeGenerator, AutoInterfaceGenerator>(source);
            return Verify(GenerationExtensions.GetGenerationOutput(output), "cs")
                .UseDirectory("Snapshots")
                .ScrubVersionNumber()
                .ScrubTimestamp();
        }

        [TestMethod]
        public Task ShouldGenerateWithGenericMethod()
        {
            const string source = """
            namespace TestNamespace
            {
                [Minerals.AutoInterfaces.AutoInterface]
                public class TestClass
                {
                    public int TestGenericMethod<T>(T arg)
                    {
                        return arg.GetHashCode();
                    }
                }
            }
            """;
            var output = GenerationExtensions.RunGeneration<AutoInterfaceAttributeGenerator, AutoInterfaceGenerator>(source);
            return Verify(GenerationExtensions.GetGenerationOutput(output), "cs")
                .UseDirectory("Snapshots")
                .ScrubVersionNumber()
                .ScrubTimestamp();
        }

        [TestMethod]
        public Task ShouldGenerateWithMultiGenericMethod()
        {
            const string source = """
            namespace TestNamespace
            {
                [Minerals.AutoInterfaces.AutoInterface]
                public class TestClass
                {
                    public int TestGenericMethod<T1, T2>(T1 arg1, T2 arg2)
                    {
                        return arg1.GetHashCode() + arg2.GetHashCode();
                    }
                }
            }
            """;
            var output = GenerationExtensions.RunGeneration<AutoInterfaceAttributeGenerator, AutoInterfaceGenerator>(source);
            return Verify(GenerationExtensions.GetGenerationOutput(output), "cs")
                .UseDirectory("Snapshots")
                .ScrubVersionNumber()
                .ScrubTimestamp();
        }

        [TestMethod]
        public Task ShouldGenerateWithConstrainedGenericMethod()
        {
            const string source = """
            using System;

            namespace TestNamespace
            {
                [Minerals.AutoInterfaces.AutoInterface]
                public class TestClass
                {
                    public bool TestGenericMethod0<T>(T arg0, T arg1) where T : unmanaged, IEquatable<T>
                    {
                        return arg0.Equals(arg1);
                    }

                    public bool TestGenericMethod1<T>(T arg0, T arg1) where T : notnull, IEquatable<T>, new()
                    {
                        return arg0.Equals(arg1);
                    }

                    public bool TestGenericMethod2<T>(T arg0, T arg1) where T : class, IEquatable<T>, new()
                    {
                        return arg0.Equals(arg1);
                    }

                    public bool TestGenericMethod3<T>(T arg0, T arg1) where T : struct, IEquatable<T>
                    {
                        return arg0.Equals(arg1);
                    }
                }
            }
            """;
            var output = GenerationExtensions.RunGeneration<AutoInterfaceAttributeGenerator, AutoInterfaceGenerator>(source);
            return Verify(GenerationExtensions.GetGenerationOutput(output), "cs")
                .UseDirectory("Snapshots")
                .ScrubVersionNumber()
                .ScrubTimestamp();
        }

        [TestMethod]
        public Task ShouldGenerateWithMultiConstrainedGenericMethod()
        {
            const string source = """
            using System;

            namespace TestNamespace
            {
                [Minerals.AutoInterfaces.AutoInterface]
                public class TestClass
                {
                    public bool TestGenericMethod0<T0, T1>(T0 arg0, T1 arg1) where T0 : unmanaged, IEquatable<T1> where T1 : unmanaged, IEquatable<T0>
                    {
                        return arg0.Equals(arg1);
                    }

                    public bool TestGenericMethod1<T0, T1>(T0 arg0, T1 arg1) where T0 : notnull, IEquatable<T1> where T1 : class, IEquatable<T0>, new()
                    {
                        return arg0.Equals(arg1);
                    }

                    public bool TestGenericMethod2<T0, T1>(T0 arg0, T1 arg1) where T0 : struct, IEquatable<T1> where T1 : notnull, IEquatable<T0>, new()
                    {
                        return arg0.Equals(arg1);
                    }
                }
            }
            """;
            var output = GenerationExtensions.RunGeneration<AutoInterfaceAttributeGenerator, AutoInterfaceGenerator>(source);
            return Verify(GenerationExtensions.GetGenerationOutput(output), "cs")
                .UseDirectory("Snapshots")
                .ScrubVersionNumber()
                .ScrubTimestamp();
        }

        [TestMethod]
        public Task ShouldGenerateWithNullableType()
        {
            const string source = """
            namespace TestNamespace
            {
                [Minerals.AutoInterfaces.AutoInterface]
                public class TestClass
                {
                    public int? Property1 { get; set; }

                    public int? TestNullableMethod(int? arg)
                    {
                        return arg.HasValue ? arg.Value + 1 : 1;
                    }
                }
            }
            """;
            var output = GenerationExtensions.RunGeneration<AutoInterfaceAttributeGenerator, AutoInterfaceGenerator>(source);
            return Verify(GenerationExtensions.GetGenerationOutput(output), "cs")
                .UseDirectory("Snapshots")
                .ScrubVersionNumber()
                .ScrubTimestamp();
        }

        [TestMethod]
        public Task ShouldGenerateWithGenericClass()
        {
            const string source = """
            namespace TestNamespace
            {
                [Minerals.AutoInterfaces.AutoInterface]
                public class TestClass<T>
                {
                    public T Property1 { get; set; }

                    public T GenericMethod(T arg)
                    {
                        return arg;
                    }
                }
            }
            """;
            var output = GenerationExtensions.RunGeneration<AutoInterfaceAttributeGenerator, AutoInterfaceGenerator>(source);
            return Verify(GenerationExtensions.GetGenerationOutput(output), "cs")
                .UseDirectory("Snapshots")
                .ScrubVersionNumber()
                .ScrubTimestamp();
        }

        [TestMethod]
        public Task ShouldGenerateWithMultiGenericClass()
        {
            const string source = """
            namespace TestNamespace
            {
                [Minerals.AutoInterfaces.AutoInterface]
                public class TestClass<T0, T1>
                {
                    public T0 Property1 { get; set; }
                    public T1 Property2 { get; set; }

                    public (T0, T1) GenericMethod(T0 arg0, T1 arg1)
                    {
                        return (arg0, arg1);
                    }
                }
            }
            """;
            var output = GenerationExtensions.RunGeneration<AutoInterfaceAttributeGenerator, AutoInterfaceGenerator>(source);
            return Verify(GenerationExtensions.GetGenerationOutput(output), "cs")
                .UseDirectory("Snapshots")
                .ScrubVersionNumber()
                .ScrubTimestamp();
        }

        [TestMethod]
        public Task ShouldGenerateWithConstrainedGenericClass()
        {
            const string source = """
            namespace TestNamespace
            {
                [Minerals.AutoInterfaces.AutoInterface]
                public class TestClass<T> where T : class, new()
                {
                    public T Property1 { get; set; } = new T();

                    public T GenericMethod(T arg)
                    {
                        return arg;
                    }
                }
            }
            """;
            var output = GenerationExtensions.RunGeneration<AutoInterfaceAttributeGenerator, AutoInterfaceGenerator>(source);
            return Verify(GenerationExtensions.GetGenerationOutput(output), "cs")
                .UseDirectory("Snapshots")
                .ScrubVersionNumber()
                .ScrubTimestamp();
        }

        [TestMethod]
        public Task ShouldGenerateWithMultiConstrainedGenericClass()
        {
            const string source = """
            using System;

            namespace TestNamespace
            {
                [Minerals.AutoInterfaces.AutoInterface]
                public class TestClass<T0, T1> where T0 : unmanaged, IEquatable<T1> where T1 : unmanaged, IEquatable<T0>
                {
                    public bool TestGenericMethod(T0 arg0, T1 arg1)
                    {
                        return arg0.Equals(arg1);
                    }
                }
            }
            """;
            var output = GenerationExtensions.RunGeneration<AutoInterfaceAttributeGenerator, AutoInterfaceGenerator>(source);
            return Verify(GenerationExtensions.GetGenerationOutput(output), "cs")
                .UseDirectory("Snapshots")
                .ScrubVersionNumber()
                .ScrubTimestamp();
        }

        [TestMethod]
        public Task ShouldGenerateWithBlockScopeNamespace()
        {
            const string source = """
            using System;
            namespace TestNamespace;

            [Minerals.AutoInterfaces.AutoInterface]
            public class TestClass
            {
                public bool TestGenericMethod0<T>(T arg0, T arg1) where T : unmanaged, IEquatable<T>
                {
                    return arg0.Equals(arg1);
                }
            }
            """;
            var output = GenerationExtensions.RunGeneration<AutoInterfaceAttributeGenerator, AutoInterfaceGenerator>(source);
            return Verify(GenerationExtensions.GetGenerationOutput(output), "cs")
                .UseDirectory("Snapshots")
                .ScrubVersionNumber()
                .ScrubTimestamp();
        }
    }
}