
namespace Minerals.AutoInterfaces.Tests
{
    [TestClass]
    public class GenerateInterfaceGeneratorTests : VerifyBase
    {
        public GenerateInterfaceGeneratorTests()
        {
            var references = VerifyExtensions.GetAppReferences
            (
                typeof(object),
                typeof(GenerateInterfaceAttributeGenerator),
                typeof(Assembly)
            );
            VerifyExtensions.Initialize(references);
        }

        [TestMethod]
        public Task ClassWithoutNamespace_ShouldGenerate()
        {
            const string source = """
            using System;
            using Minerals.AutoInterfaces;

            [GenerateInterface]
            public class TestClass
            {
                public int Property1 { get; set; }
            }
            """;
            IIncrementalGenerator[] additional =
            [
                new GenerateInterfaceAttributeGenerator()
            ];
            return this.VerifyIncrementalGenerators(source, new GenerateInterfaceGenerator(), additional);
        }

        [TestMethod]
        public Task Properties_ShouldGenerateOnlyPublic()
        {
            const string source = """
            using System;
            using Minerals.AutoInterfaces;

            namespace Minerals.Examples
            {
                [GenerateInterface]
                public class TestClass
                {
                    public int Property1 { get; set; }
                    protected int Property2 {get; set; }
                    internal int Property3 { get; set; }
                    private int Property4 { get; set; }
                }
            }
            """;
            IIncrementalGenerator[] additional =
            [
                new GenerateInterfaceAttributeGenerator()
            ];
            return this.VerifyIncrementalGenerators(source, new GenerateInterfaceGenerator(), additional);
        }

        [TestMethod]
        public Task Properties_ShouldGenerateWithoutAttributes()
        {
            const string source = """
            using System;
            using Minerals.AutoInterfaces;

            namespace Minerals.Examples
            {
                [GenerateInterface]
                public class TestClass
                {
                    [Obsolete]
                    public int Property1 { get; set; }
                    [Obsolete] public int Property2 { get; set; }
                    public float Property3 { get; [Obsolete] set; }
                }
            }
            """;
            IIncrementalGenerator[] additional =
            [
                new GenerateInterfaceAttributeGenerator()
            ];
            return this.VerifyIncrementalGenerators(source, new GenerateInterfaceGenerator(), additional);
        }

        [TestMethod]
        public Task PropertiesAccessors_ShouldGenerateWithoutAssigments()
        {
            const string source = """
            using System;
            using Minerals.AutoInterfaces;

            namespace Minerals.Examples
            {
                [GenerateInterface]
                public class TestClass
                {
                    public int Property1 { get; set; } = 1;
                    public int Property2 { get; } = 2;
                    public int Property3 { get; init; } = 3;
                }
            }
            """;
            IIncrementalGenerator[] additional =
            [
                new GenerateInterfaceAttributeGenerator()
            ];
            return this.VerifyIncrementalGenerators(source, new GenerateInterfaceGenerator(), additional);
        }

        [TestMethod]
        public Task PropertiesAccessors_ShouldGenerateOnlyPublic()
        {
            const string source = """
            using System;
            using Minerals.AutoInterfaces;

            namespace Minerals.Examples
            {
                [GenerateInterface]
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
            IIncrementalGenerator[] additional =
            [
                new GenerateInterfaceAttributeGenerator()
            ];
            return this.VerifyIncrementalGenerators(source, new GenerateInterfaceGenerator(), additional);
        }

        [TestMethod]
        public Task PropertiesAccessors_ShouldGenerateWithoutBody()
        {
            const string source = """
            using System;
            using Minerals.AutoInterfaces;

            namespace Minerals.Examples
            {
                [GenerateInterface]
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
            IIncrementalGenerator[] additional =
            [
                new GenerateInterfaceAttributeGenerator()
            ];
            return this.VerifyIncrementalGenerators(source, new GenerateInterfaceGenerator(), additional);
        }

        [TestMethod]
        public Task Fields_ShouldNotGenerate()
        {
            const string source = """
            using System;
            using Minerals.AutoInterfaces;

            namespace Minerals.Examples
            {
                [GenerateInterface]
                public class TestClass
                {
                    public int Field1 = 1;
                    protected float Field2 = 2f;
                    internal int Field3 = 3;
                    private int Field4;
                }
            }
            """;
            IIncrementalGenerator[] additional =
            [
                new GenerateInterfaceAttributeGenerator()
            ];
            return this.VerifyIncrementalGenerators(source, new GenerateInterfaceGenerator(), additional);
        }

        [TestMethod]
        public Task EventFields_ShouldGenerate()
        {
            const string source = """
            using System;
            using Minerals.AutoInterfaces;

            namespace Minerals.Examples
            {
                [GenerateInterface]
                public class TestClass
                {
                    public event EventHandler? Event1;
                    public event Action<int>? Event2;
                }
            }
            """;
            IIncrementalGenerator[] additional =
            [
                new GenerateInterfaceAttributeGenerator()
            ];
            return this.VerifyIncrementalGenerators(source, new GenerateInterfaceGenerator(), additional);
        }

        [TestMethod]
        public Task Events_ShouldGenerate()
        {
            const string source = """
            using System;
            using Minerals.AutoInterfaces;

            namespace Minerals.Examples
            {
                [GenerateInterface]
                public class TestClass
                { 
                    public EventHandler? Event1 { get; set; }
                    public Action<int>? Event2 { get; set; }
                }
            }
            """;
            IIncrementalGenerator[] additional =
            [
                new GenerateInterfaceAttributeGenerator()
            ];
            return this.VerifyIncrementalGenerators(source, new GenerateInterfaceGenerator(), additional);
        }

        [TestMethod]
        public Task Methods_ShouldGenerate()
        {
            const string source = """
            using System;
            using Minerals.AutoInterfaces;

            namespace Minerals.Examples
            {
                [GenerateInterface]
                public class TestClass
                {
                    public void Method1() { }
                    public void Method2(int arg) { }
                    public int Method3() => 3;
                    public int Method4(int arg) => arg;
                }
            }
            """;
            IIncrementalGenerator[] additional =
            [
                new GenerateInterfaceAttributeGenerator()
            ];
            return this.VerifyIncrementalGenerators(source, new GenerateInterfaceGenerator(), additional);
        }

        [TestMethod]
        public Task Methods_ShouldGenerateWithoutBody()
        {
            const string source = """
            using System;
            using Minerals.AutoInterfaces;

            namespace Minerals.Examples
            {
                [GenerateInterface]
                public class TestClass
                {
                    public void Method1()
                    {
                        Console.WriteLine("Hello, World!");
                    }
                }
            }
            """;
            IIncrementalGenerator[] additional =
            [
                new GenerateInterfaceAttributeGenerator()
            ];
            return this.VerifyIncrementalGenerators(source, new GenerateInterfaceGenerator(), additional);
        }

        [TestMethod]
        public Task Methods_ShouldGenerateWithoutAttributes()
        {
            const string source = """
            using System;
            using Minerals.AutoInterfaces;

            namespace Minerals.Examples
            {
                [GenerateInterface]
                public class TestClass
                {
                    [Obsolete]
                    public void Method1() { }
                    [Obsolete] public void Method2(int arg) { }
                }
            }
            """;
            IIncrementalGenerator[] additional =
            [
                new GenerateInterfaceAttributeGenerator()
            ];
            return this.VerifyIncrementalGenerators(source, new GenerateInterfaceGenerator(), additional);
        }

        [TestMethod]
        public Task Methods_ShouldGenerateOnlyPublic()
        {
            const string source = """
            using System;
            using Minerals.AutoInterfaces;

            namespace Minerals.Examples
            {
                [GenerateInterface]
                public class TestClass
                {
                    public void Method1() { }
                    protected void Method2(int arg) { }
                    internal void Method3() { }
                    private void Method4(int arg) { }
                }
            }
            """;
            IIncrementalGenerator[] additional =
            [
                new GenerateInterfaceAttributeGenerator()
            ];
            return this.VerifyIncrementalGenerators(source, new GenerateInterfaceGenerator(), additional);
        }

        [TestMethod]
        public Task Members_ShouldGenerateWithoutStatic()
        {
            const string source = """
            using System;
            using Minerals.AutoInterfaces;

            namespace Minerals.Examples
            {
                [GenerateInterface]
                public class TestClass
                {
                    public static int Property1 { get; set; } = 1;
                    public int Property2 { get; set; } = 2;
                    
                    public static event Action<int>? Event1;
                    public static int Field1 = 1;

                    public static void Method1() { }
                    public void Method2() { }
                }
            }
            """;
            IIncrementalGenerator[] additional =
            [
                new GenerateInterfaceAttributeGenerator()
            ];
            return this.VerifyIncrementalGenerators(source, new GenerateInterfaceGenerator(), additional);
        }

        [TestMethod]
        public Task Usings_ShouldGenerateWithoutGlobal()
        {
            const string source = """
            using System;
            global using System.Linq;
            using Minerals.AutoInterfaces;

            namespace Minerals.Examples
            {
                [GenerateInterface]
                public class TestClass
                {
                    public void Method1() { }
                    protected void Method2(int arg) { }
                    internal void Method3() { }
                    private void Method4(int arg) { }
                }
            }
            """;
            IIncrementalGenerator[] additional =
            [
                new GenerateInterfaceAttributeGenerator()
            ];
            return this.VerifyIncrementalGenerators(source, new GenerateInterfaceGenerator(), additional);
        }
    }
}