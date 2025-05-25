using System.Runtime.CompilerServices;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Minerals.AutoInterfaces.Objects
{
    public readonly struct InterfaceObject : IEquatable<InterfaceObject>
    {
        public readonly string Namespace;
        public readonly string Modifier;
        public readonly string TypeName;
        public readonly string TypeArguments;
        public readonly string CustomName;
        public readonly ImmutableArray<string> Members;

        public InterfaceObject(GeneratorAttributeSyntaxContext context)
        {
            Namespace = GetNamespace(context.TargetSymbol);
            Modifier = GetModifier(context.TargetSymbol);
            TypeName = GetTypeName(context.TargetSymbol);
            TypeArguments = GetTypeArguments(context.TargetSymbol);
            CustomName = GetCustomName(context.Attributes[0]);
            Members = GetMembers(context).ToImmutableArray();
        }

        public bool Equals(InterfaceObject other)
        {
            return Namespace.Equals(other.Namespace)
                && Modifier.Equals(other.Modifier)
                && TypeName.Equals(other.TypeName)
                && TypeArguments.Equals(other.TypeArguments)
                && CustomName.Equals(other.CustomName)
                && Members.SequenceEqual(other.Members);
        }

        public override bool Equals(object obj)
        {
            return obj is InterfaceObject other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Namespace, Modifier, TypeName, TypeArguments, CustomName, Members);
        }

        private static string GetNamespace(ISymbol targetSymbol)
        {
            if (!targetSymbol.ContainingNamespace.CanBeReferencedByName)
            {
                return string.Empty;
            }
            return GetFullName(targetSymbol.ContainingNamespace);
        }

        private static string GetModifier(ISymbol targetSymbol)
        {
            return targetSymbol.DeclaredAccessibility switch
            {
                Accessibility.Public => "public",
                Accessibility.Private => "private",
                Accessibility.Protected => "protected",
                Accessibility.Internal => "internal",
                _ => "public"
            };
        }

        private static string GetTypeName(ISymbol targetSymbol)
        {
            return targetSymbol.Name;
        }

        private static string GetTypeArguments(ISymbol targetSymbol)
        {
            if (targetSymbol is INamedTypeSymbol namedType && namedType.IsGenericType)
            {
                if (TryGetTypeParametersConstraints(namedType.TypeParameters, out var constraints))
                {
                    return $"<{string.Join(", ", namedType.TypeArguments.Select(GetFullName))}>{constraints}";
                }
                return $"<{string.Join(", ", namedType.TypeArguments.Select(GetFullName))}>";
            }
            return string.Empty;
        }

        private static string GetCustomName(AttributeData attributeData)
        {
            if (attributeData.ConstructorArguments.Length > 0 && attributeData.ConstructorArguments[0].Value is string customName)
            {
                return customName;
            }
            return string.Empty;
        }

        private static IEnumerable<string> GetMembers(GeneratorAttributeSyntaxContext context)
        {
            var named = (INamedTypeSymbol)context.TargetSymbol;
            var symbols = named.GetMembers().Where(x => x.DeclaredAccessibility is Accessibility.Public)
                .Where(x => x.Kind is SymbolKind.Method or SymbolKind.Property or SymbolKind.Event)
                .Where(x => !x.IsStatic && !x.IsOverride && !x.IsImplicitlyDeclared);

            foreach (var symbol in symbols)
            {
                if (symbol is IMethodSymbol method)
                {
                    if (TryGetFormattedConstrainedGenericMethod(method, out var formattedMethod))
                    {
                        yield return formattedMethod;
                        continue;
                    }
                    if (TryGetFormattedGenericMethod(method, out formattedMethod))
                    {
                        yield return formattedMethod;
                        continue;
                    }
                    if (TryGetFormattedParameterlessMethod(method, out formattedMethod))
                    {
                        yield return formattedMethod;
                        continue;
                    }
                    if (TryGetFormattedMethod(method, out formattedMethod))
                    {
                        yield return formattedMethod;
                        continue;
                    }
                }

                if (symbol is IPropertySymbol prop)
                {
                    if (TryGetFormattedProperty(prop, out var formattedProperty))
                    {
                        yield return formattedProperty;
                        continue;
                    }
                }

                if (symbol is IEventSymbol evt)
                {
                    if (TryGetFormattedEvent(evt, out var formattedEvent))
                    {
                        yield return formattedEvent;
                        continue;
                    }
                }
            }
        }

        private static bool TryGetFormattedProperty(IPropertySymbol symbol, out string formatted)
        {
            if (symbol.IsIndexer)
            {
                formatted = string.Empty;
                return false;
            }

            var typeText = GetFullName(symbol.Type);
            var getterIsPublic = symbol.GetMethod?.DeclaredAccessibility.HasFlag(Accessibility.Public) ?? false;
            var setterIsPublic = symbol.SetMethod?.DeclaredAccessibility.HasFlag(Accessibility.Public) ?? false;
            var setterIsInit = symbol.SetMethod?.IsInitOnly ?? false;

            if (getterIsPublic && setterIsPublic)
            {
                if (setterIsInit)
                {
                    formatted = $"{typeText} {symbol.Name} {{ get; init; }}";
                    return true;
                }
                formatted = $"{typeText} {symbol.Name} {{ get; set; }}";
                return true;
            }
            else if (setterIsPublic)
            {
                if (setterIsInit)
                {
                    formatted = $"{typeText} {symbol.Name} {{ init; }}";
                    return true;
                }
                formatted = $"{typeText} {symbol.Name} {{ set; }}";
                return true;
            }
            else if (getterIsPublic)
            {
                formatted = $"{typeText} {symbol.Name} {{ get; }}";
                return true;
            }

            formatted = string.Empty;
            return false;
        }

        private static bool TryGetFormattedConstrainedGenericMethod(IMethodSymbol sym, out string formatted)
        {
            if (sym.MethodKind is not MethodKind.Ordinary || sym.IsExtensionMethod || sym.TypeParameters.Length == 0)
            {
                formatted = string.Empty;
                return false;
            }
            if (!TryGetTypeParametersConstraints(sym.TypeParameters, out var constraints))
            {
                formatted = string.Empty;
                return false;
            }
            var returnType = GetFullName(sym.ReturnType);
            var typeParams = string.Join(", ", sym.TypeParameters.Select(GetFullName));
            var parameters = string.Join(", ", sym.Parameters.Select(x => $"{GetFullName(x.Type)} {x.Name}"));
            formatted = $"{returnType} {sym.Name}<{typeParams}>({parameters}){constraints};";
            return true;
        }

        private static bool TryGetFormattedGenericMethod(IMethodSymbol sym, out string formatted)
        {
            if (sym.MethodKind is not MethodKind.Ordinary || sym.IsExtensionMethod || sym.TypeParameters.Length == 0)
            {
                formatted = string.Empty;
                return false;
            }

            var returnType = GetFullName(sym.ReturnType);
            var typeParams = string.Join(", ", sym.TypeParameters.Select(GetFullName));
            var parameters = string.Join(", ", sym.Parameters.Select(x => $"{GetFullName(x.Type)} {x.Name}"));
            formatted = $"{returnType} {sym.Name}<{typeParams}>({parameters});";
            return true;
        }

        private static bool TryGetFormattedParameterlessMethod(IMethodSymbol sym, out string formatted)
        {
            if (sym.MethodKind is not MethodKind.Ordinary || sym.IsExtensionMethod || sym.Parameters.Length > 0)
            {
                formatted = string.Empty;
                return false;
            }

            var returnType = GetFullName(sym.ReturnType);
            formatted = $"{returnType} {sym.Name}();";
            return true;
        }

        private static bool TryGetFormattedMethod(IMethodSymbol sym, out string formatted)
        {
            if (sym.MethodKind is not MethodKind.Ordinary || sym.IsExtensionMethod)
            {
                formatted = string.Empty;
                return false;
            }

            var returnType = GetFullName(sym.ReturnType);
            var parameters = string.Join(", ", sym.Parameters.Select(x => $"{GetFullName(x.Type)} {x.Name}"));
            formatted = $"{returnType} {sym.Name}({parameters});";
            return true;
        }

        private static bool TryGetFormattedEvent(IEventSymbol sym, out string formatted)
        {
            var typeText = GetFullName(sym.Type);
            formatted = $"{typeText} {sym.Name};";
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryGetTypeParametersConstraints(IEnumerable<ITypeParameterSymbol> symbols, out string constraints)
        {
            var typeParams = symbols.Where(x => x.ConstraintTypes.Length > 0
                || x.HasReferenceTypeConstraint
                || x.HasConstructorConstraint
                || x.HasValueTypeConstraint
                || x.HasUnmanagedTypeConstraint
                || x.HasNotNullConstraint);

            if (!typeParams.Any())
            {
                constraints = string.Empty;
                return false;
            }

            var builder = new StringBuilder(128);
            foreach (var typeParam in typeParams)
            {
                AppendTypeParameterConstraints(typeParam, builder);
            }

            constraints = builder.ToString();
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AppendTypeParameterConstraints(ITypeParameterSymbol sym, StringBuilder builder)
        {
            builder.Append(" where ").Append(sym.Name).Append(" : ");

            var hasAnyPredefinedConstraint = false;
            if (sym.HasReferenceTypeConstraint)
            {
                builder.Append("class");
                hasAnyPredefinedConstraint = true;
            }
            else if (sym.HasUnmanagedTypeConstraint)
            {
                builder.Append("unmanaged");
                hasAnyPredefinedConstraint = true;
            }
            else if (sym.HasValueTypeConstraint)
            {
                builder.Append("struct");
                hasAnyPredefinedConstraint = true;
            }
            else if (sym.HasNotNullConstraint)
            {
                builder.Append("notnull");
                hasAnyPredefinedConstraint = true;
            }

            if (sym.ConstraintTypes.Length > 0)
            {
                if (hasAnyPredefinedConstraint)
                {
                    builder.Append(", ");
                }
                foreach (var constraintType in sym.ConstraintTypes)
                {
                    builder.Append(GetFullName(constraintType));
                }
            }

            if (sym.HasConstructorConstraint)
            {
                if (hasAnyPredefinedConstraint || sym.ConstraintTypes.Length > 0)
                {
                    builder.Append(", ");
                }
                builder.Append("new()");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetFullName(ISymbol sym)
        {
            return sym.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }
    }
}