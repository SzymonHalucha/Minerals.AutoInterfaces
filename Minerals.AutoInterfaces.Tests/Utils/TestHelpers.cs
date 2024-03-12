namespace Minerals.AutoInterfaces.Tests.Utils
{
    public static class TestHelpers
    {
        [ModuleInitializer]
        public static void Initialize()
        {
            DiffTools.UseOrder(DiffTool.VisualStudioCode, DiffTool.VisualStudio, DiffTool.Rider);
            UseProjectRelativeDirectory("Snapshots");
            VerifierSettings.UseEncoding(Encoding.UTF8);
            VerifySourceGenerators.Initialize();
        }

        public static Task VerifyGenerator(string source, IIncrementalGenerator generator, IIncrementalGenerator[] additional)
        {
            var tree = CSharpSyntaxTree.ParseText(source);
            var compilation = CSharpCompilation.Create
            (
                "Tests",
                [tree],
                [MetadataReference.CreateFromFile(generator.GetType().Assembly.Location)]
            );

            CSharpGeneratorDriver.Create(additional)
                .RunGeneratorsAndUpdateCompilation
                (
                    compilation,
                    out var newCompilation,
                    out _
                );

            var driver = CSharpGeneratorDriver.Create(generator)
                .RunGenerators(newCompilation);

            return Verify(driver);
        }

        public static Task VerifyGenerator(IIncrementalGenerator generator, IIncrementalGenerator[] additional)
        {
            var compilation = CSharpCompilation.Create("Tests");
            CSharpGeneratorDriver.Create(additional)
                .RunGeneratorsAndUpdateCompilation
                (
                    compilation,
                    out var newCompilation,
                    out _
                );

            var driver = CSharpGeneratorDriver.Create(generator)
                .RunGenerators(newCompilation);

            return Verify(driver);
        }

        public static string MakeTestClassWithNamespace(string additionalCode)
        {
            return $$"""
            using System;
            using System.Text;
            using System.Linq;
            using System.Collections.Generic;
            using Minerals.AutoInterfaces;

            namespace Minerals.AutoInterfaces.Tests
            {
                [GenerateInterface]
                public class TestClass
                {
                    {{additionalCode}}
                }
            }
            """;
        }
    }
}