namespace Minerals.AutoInterfaces
{
    [Generator]
    public class GenerateInterfaceGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var generates = context.SyntaxProvider.ForAttributeWithMetadataName
            (
                "Minerals.AutoInterfaces.GenerateInterfaceAttribute",
                static (x, _) => true, //TODO: Optimize this for value types that matters
                static (x, _) => x
            );

            context.RegisterSourceOutput(generates, static (ctx, element) =>
            {
                string fileName = $"I{element.GetTargetNodeName()}.g.cs";
                ctx.AddSource(fileName, BuildInterfaceForTargetNode(element));
            });
        }

        public static SourceText BuildInterfaceForTargetNode(GeneratorAttributeSyntaxContext context)
        {
            var builder = new CodeBuilder();
            builder.AddAutoGeneratedHeader(Assembly.GetExecutingAssembly());

            builder.AddUsingsFrom(context.TargetNode);
            builder.AddNamespaceDeclarationHeader(context.TargetNode);
            builder.AddAutoGeneratedAttributes(typeof(InterfaceDeclarationSyntax));

            GenerateInterface(builder, context.TargetNode);
            GenerateAllPublicMembers(builder, context.TargetNode);
            builder.CloseAllBlocks();

            return SourceText.From(builder.ToString(), Encoding.UTF8);
        }

        public static void GenerateInterface(CodeBuilder builder, SyntaxNode node)
        {
            builder.WriteLine(node.GetAccessModifier().ToString())
                .Write(" interface I")
                .Write(((BaseTypeDeclarationSyntax)node).Identifier.ValueText)
                .OpenBlock();
        }

        public static void GenerateAllPublicMembers(CodeBuilder builder, SyntaxNode node)
        {
            var cls = (TypeDeclarationSyntax)node;
            var members = cls.Members.Where(x => x.HasModifier(SyntaxKind.PublicKeyword));
            foreach (var member in members)
            {
                if (member is MethodDeclarationSyntax method)
                {
                    GenerateMethodHeader(builder, method);
                }
                if (member is EventFieldDeclarationSyntax evt)
                {
                    GenerateEventFieldHeader(builder, evt);
                }
                if (member is PropertyDeclarationSyntax property)
                {
                    GeneratePropertyHeader(builder, property);
                }
            }
        }

        public static void GenerateMethodHeader(CodeBuilder builder, MethodDeclarationSyntax method)
        {
            builder.WriteLine(method.ReturnType.ToString())
                .Write(" ")
                .Write(method.Identifier.ValueText);

            if (method.TypeParameterList != null)
            {
                builder.Write(method.TypeParameterList!.ToString());
            }

            builder.Write(method.ParameterList.ToString());

            if (method.ConstraintClauses.Count > 0)
            {
                builder.Write(" ");
                builder.Write(method.ConstraintClauses.ToString());
            }

            builder.Write(";");
        }

        public static void GenerateEventFieldHeader(CodeBuilder builder, EventFieldDeclarationSyntax evt)
        {
            builder.WriteLine("event ")
                .Write(evt.Declaration.Type.ToString())
                .Write(" ")
                .Write(evt.Declaration.Variables.ToString())
                .Write(";");
        }

        public static void GeneratePropertyHeader(CodeBuilder builder, PropertyDeclarationSyntax property)
        {
            builder.WriteLine(property.Type.ToString())
                .Write(" ")
                .Write(property.Identifier.ValueText)
                .Write(" {");

            if (property.HasPublicAccessor(SyntaxKind.GetAccessorDeclaration))
            {
                builder.Write(" get;");
            }
            if (property.HasPublicAccessor(SyntaxKind.SetAccessorDeclaration))
            {
                builder.Write(" set;");
            }
            if (property.HasPublicAccessor(SyntaxKind.InitAccessorDeclaration))
            {
                builder.Write(" init;");
            }

            builder.Write(" }");
        }
    }
}