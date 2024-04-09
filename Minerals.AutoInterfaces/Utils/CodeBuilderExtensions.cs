namespace Minerals.AutoInterfaces.Utils
{
    public static class CodeBuilderExtensions
    {
        public static CodeBuilder AddAutoGeneratedHeader(this CodeBuilder builder, Assembly assembly)
        {
            var title = assembly.GetCustomAttribute<AssemblyTitleAttribute>().Title;
            var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            builder.Write("// <auto-generated>");
            builder.WriteLine("// This code was generated by a tool.");
            builder.WriteLine("// Name: ").Write(title);
            builder.WriteLine("// Version: ").Write(version);
            builder.WriteLine("// </auto-generated>");
            return builder;
        }

        public static CodeBuilder AddAutoGeneratedAttributes(this CodeBuilder builder, Type type)
        {
            if (type == typeof(ClassDeclarationSyntax)
            || type == typeof(StructDeclarationSyntax)
            || type == typeof(RecordDeclarationSyntax))
            {
                builder.WriteLine("[global::System.Diagnostics.DebuggerNonUserCode]");
                builder.WriteLine("[global::System.Runtime.CompilerServices.CompilerGenerated]");
                builder.WriteLine("[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]");
            }
            else
            {
                builder.WriteLine("[global::System.Runtime.CompilerServices.CompilerGenerated]");
            }
            return builder;
        }

        public static CodeBuilder AddUsingsFrom(this CodeBuilder builder, SyntaxNode from)
        {
            var usings = from.FirstAncestorOrSelf<CompilationUnitSyntax>()?.Usings;
            if (usings != null)
            {
                foreach (var item in usings)
                {
                    builder.WriteLine(item.ToString());
                }
                builder.NewLine();
            }
            return builder;
        }

        public static CodeBuilder AddUsingsFrom(this CodeBuilder builder, IEnumerable<SyntaxNode> nodes)
        {
            var usings = new HashSet<string>();
            foreach (var node in nodes)
            {
                var list = node.FirstAncestorOrSelf<CompilationUnitSyntax>()?.Usings;
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        usings.Add(item.Name!.ToString());
                    }
                }
            }
            if (usings.Count > 0)
            {
                foreach (var item in usings)
                {
                    builder.WriteLine("using ").Write(item).Write(";");
                }
                builder.NewLine();
            }
            return builder;
        }

        public static CodeBuilder AddNamespaceDeclarationHeader(this CodeBuilder builder, SyntaxNode namespaceFrom)
        {
            var ns = namespaceFrom.FirstAncestorOrSelf<NamespaceDeclarationSyntax>();
            if (ns != null)
            {
                builder.WriteLine("namespace ").Write(ns.Name.ToString()).OpenBlock();
            }
            return builder;
        }

        public static CodeBuilder AddTypeDeclarationHeader(this CodeBuilder builder, SyntaxNode node)
        {
            return builder.AddTypeDeclarationHeader(node, Array.Empty<string>());
        }

        public static CodeBuilder AddTypeDeclarationHeader(this CodeBuilder builder, SyntaxNode node, string[] interfaces)
        {
            var typeSyntax = (TypeDeclarationSyntax)node;
            builder.WriteLine(typeSyntax.Modifiers.ToString())
                .Write(" ")
                .Write(typeSyntax.Keyword.ToString())
                .Write(" ")
                .Write(typeSyntax.Identifier.ValueText);

            if (typeSyntax.TypeParameterList != null)
            {
                builder.Write(typeSyntax.TypeParameterList.ToString());
            }
            if (typeSyntax.ParameterList != null)
            {
                builder.Write(typeSyntax.ParameterList.ToString());
            }
            if (typeSyntax.BaseList != null)
            {
                builder.Write(typeSyntax.BaseList.ToString());
            }
            if (interfaces.Length > 0)
            {
                if (typeSyntax.BaseList == null)
                {
                    builder.Write(" : ");
                }
                else
                {
                    builder.Write(", ");
                }
                for (int i = 0; i < interfaces.Length; i++)
                {
                    builder.Write(interfaces[i]);
                    if (i < interfaces.Length - 1)
                    {
                        builder.Write(", ");
                    }
                }
            }
            if (typeSyntax.ConstraintClauses.Count > 0)
            {
                builder.Write(typeSyntax.ConstraintClauses.ToString());
            }
            return builder;
        }
    }
}