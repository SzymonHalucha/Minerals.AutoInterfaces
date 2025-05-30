using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using Minerals.AutoInterfaces.Utilities;

namespace Minerals.AutoInterfaces
{
    [Generator]
    public class AutoInterfaceAttributeGenerator : IIncrementalGenerator
    {
        public const string FileName = "AutoInterfaceAttribute.g.cs";

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterPostInitializationOutput(static (ctx) => ctx.AddSource(FileName, GenerateAttribute()));
        }

        private static SourceText GenerateAttribute()
        {
            const string source = """
            #pragma warning disable CS9113
            namespace Minerals.AutoInterfaces
            {
                [global::System.Diagnostics.DebuggerNonUserCode]
                [global::System.Runtime.CompilerServices.CompilerGenerated]
                [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
                [global::System.AttributeUsage(global::System.AttributeTargets.Class | global::System.AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
                public sealed class AutoInterfaceAttribute : global::System.Attribute
                {
                    public AutoInterfaceAttribute(string customName = "")
                    {
                    }
                }
            }
            #pragma warning restore CS9113
            """;
            var builder = new CodeBuilder();
            builder.AppendAutoGeneratedHeader().WriteLine(source);
            return SourceText.From(builder.ToString(), System.Text.Encoding.UTF8);
        }
    }
}