namespace Minerals.AutoInterfaces.Tests
{
    public class InterfaceGeneratorTests
    {
        [Fact]
        public Task ClassWithoutNamespace_ShouldGenerate()
        {
            const string source = """
            public class TestClass
            {
                public int Property1 { get; set; }
            }
            """;
            return TestHelpers.VerifyGenerator(source, new InterfaceGenerator(), [new GenerateInterfaceAttributeGenerator()]);
        }

        [Fact]
        public Task Properties_ShouldGenerateOnlyPublic()
        {
            string source = TestHelpers.MakeTestClassWithNamespace
            (
                """
                public int Property1 { get; set; }
                protected int Property2 {get; set; }
                internal int Property3 { get; set; }
                private int Property4 { get; set; }
                """
            );
            return TestHelpers.VerifyGenerator(source, new InterfaceGenerator(), [new GenerateInterfaceAttributeGenerator()]);
        }

        [Fact]
        public Task Properties_ShouldGenerateWithoutAttributes()
        {
            string source = TestHelpers.MakeTestClassWithNamespace
            (
                """
                [Obsolete]
                public int Property1 { get; set; }
                [Obsolete] public int Property2 { get; set; }
                public float Property3 { get; [Obsolete] set; }
                """
            );
            return TestHelpers.VerifyGenerator(source, new InterfaceGenerator(), [new GenerateInterfaceAttributeGenerator()]);
        }

        [Fact]
        public Task PropertiesAccessors_ShouldGenerateWithoutAssigments()
        {
            string source = TestHelpers.MakeTestClassWithNamespace
            (
                """
                public int Property1 { get; set; } = 1;
                public int Property2 { get; } = 2;
                public int Property5 { get; init; } = 5;
                """
            );
            return TestHelpers.VerifyGenerator(source, new InterfaceGenerator(), [new GenerateInterfaceAttributeGenerator()]);
        }

        [Fact]
        public Task PropertiesAccessors_ShouldGenerateOnlyPublic()
        {
            string source = TestHelpers.MakeTestClassWithNamespace
            (
                """
                public int Property1 { get; set; }
                public int Property2 { get; }
                public int Property3 { private get; set; }
                public int Property4 { get; protected set; }
                public string Property5 { get; init; }
                public float Property6 { get; internal init; }
                """
            );
            return TestHelpers.VerifyGenerator(source, new InterfaceGenerator(), [new GenerateInterfaceAttributeGenerator()]);
        }

        [Fact]
        public Task PropertiesAccessors_ShouldGenerateWithoutBody()
        {
            string source = TestHelpers.MakeTestClassWithNamespace
            (
                """
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
                """
            );
            return TestHelpers.VerifyGenerator(source, new InterfaceGenerator(), [new GenerateInterfaceAttributeGenerator()]);
        }

        [Fact]
        public Task Fields_ShouldNotGenerate()
        {
            string source = TestHelpers.MakeTestClassWithNamespace
            (
                """
                public int Field1 = 1;
                protected float Field2 = 2f;
                internal int Field3 = 3;
                private int Field4;
                """
            );
            return TestHelpers.VerifyGenerator(source, new InterfaceGenerator(), [new GenerateInterfaceAttributeGenerator()]);
        }

        [Fact]
        public Task Events_ShouldGenerate()
        {
            string source = TestHelpers.MakeTestClassWithNamespace
            (
                """
                public event EventHandler? Event1;
                public event Action<int>? Event2;
                """
            );
            return TestHelpers.VerifyGenerator(source, new InterfaceGenerator(), [new GenerateInterfaceAttributeGenerator()]);
        }

        [Fact]
        public Task Methods_ShouldGenerate()
        {
            string source = TestHelpers.MakeTestClassWithNamespace
            (
                """
                public void Method1() { }
                public void Method2(int arg) { }
                public int Method3() => 3;
                public int Method4(int arg) => arg;
                """
            );
            return TestHelpers.VerifyGenerator(source, new InterfaceGenerator(), [new GenerateInterfaceAttributeGenerator()]);
        }

        [Fact]
        public Task Methods_ShouldGenerateWithoutBody()
        {
            string source = TestHelpers.MakeTestClassWithNamespace
            (
                """
                public void Method1()
                {
                    Console.WriteLine("Hello, World!");
                }
                """
            );
            return TestHelpers.VerifyGenerator(source, new InterfaceGenerator(), [new GenerateInterfaceAttributeGenerator()]);
        }

        [Fact]
        public Task Methods_ShouldGenerateWithoutAttributes()
        {
            string source = TestHelpers.MakeTestClassWithNamespace
            (
                """
                [Obsolete]
                public void Method1() { }
                [Obsolete] public void Method2(int arg) { }
                """
            );
            return TestHelpers.VerifyGenerator(source, new InterfaceGenerator(), [new GenerateInterfaceAttributeGenerator()]);
        }

        [Fact]
        public Task Methods_ShouldGenerateOnlyPublic()
        {
            string source = TestHelpers.MakeTestClassWithNamespace
            (
                """
                public void Method1() { }
                protected void Method2(int arg) { }
                internal void Method3() { }
                private void Method4(int arg) { }
                """
            );
            return TestHelpers.VerifyGenerator(source, new InterfaceGenerator(), [new GenerateInterfaceAttributeGenerator()]);
        }
    }
}