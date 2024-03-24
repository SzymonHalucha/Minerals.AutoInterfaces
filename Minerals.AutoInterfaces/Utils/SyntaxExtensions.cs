namespace Minerals.AutoInterfaces.Utils
{
    public static class SyntaxExtensions
    {
        public static string GetTargetNodeName(this GeneratorAttributeSyntaxContext context)
        {
            return ((BaseTypeDeclarationSyntax)context.TargetNode).Identifier.ValueText;
        }

        public static NamespaceDeclarationSyntax? GetNamespace(this SyntaxNode node)
        {
            return node.FirstAncestorOrSelf<NamespaceDeclarationSyntax>();
        }

        public static CompilationUnitSyntax? GetCompilationUnit(this SyntaxNode node)
        {
            return node.FirstAncestorOrSelf<CompilationUnitSyntax>();
        }

        public static bool HasModifier(this SyntaxNode node, SyntaxKind kind)
        {
            return (node as MemberDeclarationSyntax)?.Modifiers.Any(x => x.IsKind(kind)) == true;
        }

        public static SyntaxToken? GetAccessModifier(this SyntaxNode node)
        {
            return (node as MemberDeclarationSyntax)?.Modifiers.First(x =>
                x.IsKind(SyntaxKind.PrivateKeyword)
                || x.IsKind(SyntaxKind.InternalKeyword)
                || x.IsKind(SyntaxKind.ProtectedKeyword)
                || x.IsKind(SyntaxKind.PublicKeyword));
        }

        public static bool HasPublicAccessor(this PropertyDeclarationSyntax property, SyntaxKind kind)
        {
            if (property.HasArrowExpression())
            {
                if (kind.Equals(SyntaxKind.GetAccessorDeclaration))
                {
                    return true;
                }
                return false;
            }
            if (property.AccessorList == null)
            {
                return false;
            }
            return property.AccessorList.Accessors.Any(x =>
            {
                return x.IsKind(kind)
                && (x.Modifiers.Count <= 0 || x.Modifiers.Any(y => y.IsKind(SyntaxKind.PublicKeyword)));
            });
        }

        public static bool HasArrowExpression(this PropertyDeclarationSyntax property)
        {
            return property.ExpressionBody != null;
        }

        public static bool HasArrowExpression(this BaseMethodDeclarationSyntax method)
        {
            return method.ExpressionBody != null;
        }

        public static bool HasArrowExpression(this AccessorDeclarationSyntax accesor)
        {
            return accesor.ExpressionBody != null;
        }
    }
}