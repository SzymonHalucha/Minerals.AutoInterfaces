namespace Minerals.AutoInterfaces.Attributes
{
    [Generator]
    public class GenerateInterfaceAttributeGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterPostInitializationOutput(static (context) =>
            {
                context.AddSource("GenerateInterfaceAttribute.g.cs", GenerateAttribute());
            });
        }

        public static SourceText GenerateAttribute()
        {
            const string source = """
            namespace Minerals.AutoInterfaces
            {
                [global::System.Diagnostics.DebuggerNonUserCode]
                [global::System.Runtime.CompilerServices.CompilerGenerated]
                [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
                [global::System.AttributeUsage(global::System.AttributeTargets.Class | global::System.AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
                public sealed class GenerateInterfaceAttribute : global::System.Attribute
                {
                }
            }
            """;
            var builder = new CodeBuilder();
            builder.AddAutoGeneratedHeader(Assembly.GetExecutingAssembly()).WriteLine(source);
            return SourceText.From(builder.ToString(), Encoding.UTF8);
        }

    }
}