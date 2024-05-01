using System.Collections.Immutable;

namespace Minerals.AutoInterfaces
{
    public readonly struct AttributeObject : IEquatable<AttributeObject>
    {
        public string AccessModifier { get; }
        public string Name { get; }
        public string CustomName { get; }
        public string Namespace { get; }
        public string[][] PublicMembers { get; }
        public string[] Usings { get; }

        public AttributeObject(GeneratorAttributeSyntaxContext context)
        {
            AccessModifier = GetAccessModifierOf(context.TargetNode);
            Name = GetNameOf(context.TargetNode);
            CustomName = GetCustomNameOf(context.Attributes);
            Namespace = GetNamespaceFrom(context.TargetNode);
            PublicMembers = GetPublicMembersFrom(context.TargetNode);
            Usings = GetUsingsFrom(context.TargetNode);
        }

        public override bool Equals(object obj)
        {
            return obj is AttributeObject attrObj
            && attrObj.AccessModifier.Equals(AccessModifier)
            && attrObj.Name.Equals(Name)
            && attrObj.CustomName.Equals(CustomName)
            && attrObj.Namespace.Equals(Namespace)
            && attrObj.PublicMembers.SequenceEqual(PublicMembers)
            && attrObj.Usings.SequenceEqual(Usings);
        }

        public bool Equals(AttributeObject other)
        {
            return other.AccessModifier.Equals(AccessModifier)
            && other.Name.Equals(Name)
            && other.CustomName.Equals(CustomName)
            && other.Namespace.Equals(Namespace)
            && other.PublicMembers.SequenceEqual(PublicMembers)
            && other.Usings.SequenceEqual(Usings);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(AccessModifier, CustomName, Name, Namespace, PublicMembers, Usings);
        }

        private static string GetAccessModifierOf(SyntaxNode node)
        {
            return ((MemberDeclarationSyntax)node).Modifiers.Where(x =>
            {
                return x.IsKind(SyntaxKind.PrivateKeyword)
                    || x.IsKind(SyntaxKind.ProtectedKeyword)
                    || x.IsKind(SyntaxKind.InternalKeyword)
                    || x.IsKind(SyntaxKind.PublicKeyword);
            }).Select(x => x.ValueText).Aggregate((x, y) => $"{x} {y}");
        }

        private static string GetNameOf(SyntaxNode node)
        {
            return ((BaseTypeDeclarationSyntax)node).Identifier.ValueText;
        }

        private string GetCustomNameOf(ImmutableArray<AttributeData> attributes)
        {
            return attributes.First().ConstructorArguments.First().Value?.ToString() ?? string.Empty;
        }

        private static string GetNamespaceFrom(SyntaxNode from)
        {
            var nameSyntax = from.FirstAncestorOrSelf<NamespaceDeclarationSyntax>()?.Name;
            nameSyntax ??= ((FileScopedNamespaceDeclarationSyntax?)from.FirstAncestorOrSelf<CompilationUnitSyntax>()?
                .ChildNodes().FirstOrDefault(x => x is FileScopedNamespaceDeclarationSyntax))?.Name;
            return nameSyntax?.ToString() ?? string.Empty;
        }

        private static string[][] GetPublicMembersFrom(SyntaxNode from)
        {
            var syntaxes = ((TypeDeclarationSyntax)from).Members.Where(x => IsValid(x));
            var members = new List<string[]>();
            foreach (var syntax in syntaxes)
            {
                if (syntax is MethodDeclarationSyntax method)
                {
                    members.Add(GetMethodHeader(method));
                }
                else if (syntax is EventFieldDeclarationSyntax evt)
                {
                    members.Add(GetEventFieldHeader(evt));
                }
                else if (syntax is PropertyDeclarationSyntax property)
                {
                    members.Add(GetPropertyHeader(property));
                }
            }
            return members.ToArray();
        }

        private static bool IsValid(MemberDeclarationSyntax node)
        {
            return node.Modifiers.Any(x => x.IsKind(SyntaxKind.PublicKeyword))
                && !node.Modifiers.Any(x => x.IsKind(SyntaxKind.StaticKeyword));
        }

        private static string[] GetMethodHeader(MethodDeclarationSyntax method)
        {
            var returnType = method.ReturnType.ToString();
            var name = method.Identifier.ToString();
            var typeParameters = method.TypeParameterList?.ToString();
            var parameters = method.ParameterList?.ToString();
            var clauses = method.ConstraintClauses.ToString();
            return [returnType, " ", name, typeParameters ?? string.Empty, parameters ?? "()", clauses.Length > 0 ? " " : "", clauses, ";"];
        }

        private static string[] GetEventFieldHeader(EventFieldDeclarationSyntax evt)
        {
            var type = evt.Declaration.Type.ToString();
            var variables = evt.Declaration.Variables.ToString();
            return ["event ", type, " ", variables, ";"];
        }

        private static string[] GetPropertyHeader(PropertyDeclarationSyntax property)
        {
            var type = property.Type.ToString();
            var name = property.Identifier.ValueText;
            var get = HasPublicAccessor(property, SyntaxKind.GetAccessorDeclaration) ? "get; " : string.Empty;
            var set = HasPublicAccessor(property, SyntaxKind.SetAccessorDeclaration) ? "set; " : string.Empty;
            var init = HasPublicAccessor(property, SyntaxKind.InitAccessorDeclaration) ? "init; " : string.Empty;
            return [type, " ", name, " { ", get, set, init, "}"];
        }

        private static bool HasPublicAccessor(PropertyDeclarationSyntax property, SyntaxKind kind)
        {
            if (property.ExpressionBody != null)
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
                return x.IsKind(kind) && (x.Modifiers.Count <= 0 || x.Modifiers.Any(y => y.IsKind(SyntaxKind.PublicKeyword)));
            });
        }

        private static string[] GetUsingsFrom(SyntaxNode from)
        {
            var usings = from.FirstAncestorOrSelf<CompilationUnitSyntax>()?.Usings
                .Where(x => x.GlobalKeyword.IsKind(SyntaxKind.None))
                .Select(x => x.Name!.ToString());
            return usings != null ? usings.ToArray() : [];
        }
    }
}