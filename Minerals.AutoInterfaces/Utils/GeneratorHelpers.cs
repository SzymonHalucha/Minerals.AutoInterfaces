namespace Minerals.AutoInterfaces.Utils
{
    public static class GeneratorHelpers
    {
        public static string GetClassName(GeneratorAttributeSyntaxContext context)
        {
            return ((ClassDeclarationSyntax)context.TargetNode).Identifier.ValueText;
        }

        public static SyntaxList<UsingDirectiveSyntax> GetAllUsingDirectives(GeneratorAttributeSyntaxContext context)
        {
            var usings = context.TargetNode
                .FirstAncestorOrSelf<CompilationUnitSyntax>()?
                .DescendantNodes()
                .OfType<UsingDirectiveSyntax>()
                .Where(x =>
                {
                    return !string.Equals
                    (
                        x.Name?.ToString(),
                        context.Attributes.First().AttributeClass?.ContainingNamespace.ToString(),
                        StringComparison.Ordinal
                    );
                });

            return usings != null ? SyntaxFactory.List(usings) : SyntaxFactory.List<UsingDirectiveSyntax>();
        }

        public static NamespaceDeclarationSyntax? GetNamespaceDeclaration(GeneratorAttributeSyntaxContext context)
        {
            var ns = context.TargetNode.FirstAncestorOrSelf<NamespaceDeclarationSyntax>();
            return ns != null ? SyntaxFactory.NamespaceDeclaration(ns.Name) : null;
        }

        public static InterfaceDeclarationSyntax GetInterfaceDeclaration(GeneratorAttributeSyntaxContext context)
        {
            var syntax = SyntaxFactory.InterfaceDeclaration($"I{GetClassName(context)}")
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                .WithMembers(GetAllPublicMemberDeclarations(context));
            return syntax;
        }

        public static MethodDeclarationSyntax GetMethodInterfaceDeclaration(MethodDeclarationSyntax method)
        {
            return SyntaxFactory.MethodDeclaration(method.ReturnType, method.Identifier)
                .WithModifiers(SyntaxFactory.TokenList(GetAccessModifier(method)))
                .WithTypeParameterList(method.TypeParameterList)
                .WithParameterList(method.ParameterList)
                .WithConstraintClauses(method.ConstraintClauses)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        }

        public static PropertyDeclarationSyntax GetPropertyInterfaceDeclaration(PropertyDeclarationSyntax prop)
        {
            return SyntaxFactory.PropertyDeclaration(prop.Type, prop.Identifier)
                .WithModifiers(SyntaxFactory.TokenList(GetAccessModifier(prop)))
                .WithAccessorList(GetAccessors(prop, SyntaxKind.PublicKeyword));
        }

        public static EventFieldDeclarationSyntax GetEventInterfaceDeclaration(EventFieldDeclarationSyntax evt)
        {
            return SyntaxFactory.EventFieldDeclaration(evt.Declaration)
                .WithModifiers(SyntaxFactory.TokenList(GetAccessModifier(evt)))
                .WithEventKeyword(evt.EventKeyword)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        }

        public static SyntaxList<MemberDeclarationSyntax> GetAllPublicMemberDeclarations(GeneratorAttributeSyntaxContext context)
        {
            var newMembers = SyntaxFactory.List<MemberDeclarationSyntax>();
            var members = context.TargetNode.DescendantNodes().OfType<MemberDeclarationSyntax>();
            foreach (MemberDeclarationSyntax member in members)
            {
                if (IsAccessKind(member))
                {
                    var newMember = GetMemberDeclaration(member);
                    if (newMember != null)
                    {
                        newMembers = newMembers.Add(newMember);
                    }
                }
            }
            return newMembers;
        }

        public static MemberDeclarationSyntax? GetMemberDeclaration(MemberDeclarationSyntax member)
        {
            if (member is MethodDeclarationSyntax method)
            {
                return GetMethodInterfaceDeclaration(method);
            }
            else if (member is PropertyDeclarationSyntax prop)
            {
                return GetPropertyInterfaceDeclaration(prop);
            }
            else if (member is EventFieldDeclarationSyntax evt)
            {
                return GetEventInterfaceDeclaration(evt);
            }
            return null;
        }

        public static AccessorListSyntax GetAccessors(PropertyDeclarationSyntax property, SyntaxKind accessKind)
        {
            if (property.ChildNodes().OfType<ArrowExpressionClauseSyntax>().Any())
            {
                return SyntaxFactory.AccessorList().AddAccessors
                (
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                );
            }
            var accessors = SyntaxFactory.AccessorList();
            if (IsAccessKind(property, accessKind) && property.AccessorList != null)
            {
                foreach (AccessorDeclarationSyntax accessor in property.AccessorList.Accessors)
                {
                    if (accessor.Modifiers.Count == 0 || accessor.Modifiers.Any(accessKind))
                    {
                        accessors = accessors.AddAccessors
                        (
                            SyntaxFactory.AccessorDeclaration(accessor.Kind())
                                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                        );
                    }
                }
            }
            return accessors;
        }

        public static SyntaxToken GetAccessModifier(MemberDeclarationSyntax member)
        {
            return member.Modifiers.First(x =>
                x.IsKind(SyntaxKind.PrivateKeyword)
                || x.IsKind(SyntaxKind.InternalKeyword)
                || x.IsKind(SyntaxKind.ProtectedKeyword)
                || x.IsKind(SyntaxKind.PublicKeyword));
        }

        public static bool IsAccessKind(MemberDeclarationSyntax member, SyntaxKind accessKind = SyntaxKind.PublicKeyword)
        {
            return member.Modifiers.Any(accessKind);
        }
    }
}