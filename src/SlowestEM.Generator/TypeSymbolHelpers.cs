using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SlowestEM.Generator
{
    internal static class TypeSymbolHelpers
    {
        internal static IEnumerable<IPropertySymbol> GetAllSettableProperties(this ITypeSymbol typeSymbol)
        {
            var result = typeSymbol
                .GetMembers()
                .Where(s => s.Kind == SymbolKind.Property).Cast<IPropertySymbol>()
                .Where(p => p.SetMethod?.DeclaredAccessibility == Accessibility.Public)
                .Union(typeSymbol.BaseType == null ? new IPropertySymbol[0] : typeSymbol.BaseType.GetAllSettableProperties());

            return result;
        }

        internal static string ToDisplayString(this Accessibility declaredAccessibility)
        {
            switch (declaredAccessibility)
            {
                case Accessibility.Private:
                    return "private";
                case Accessibility.Public:
                    return "public";
                case Accessibility.Internal:
                default:
                    return "internal";
            }
        }

        internal static bool IsNullable(this ITypeSymbol symbol)
        {
            return symbol is INamedTypeSymbol namedType
                && namedType.IsValueType
                && namedType.IsGenericType
                && namedType.ConstructedFrom?.ToDisplayString() == "System.Nullable<T>";
        }

        internal static string ToRealTypeDisplayString(this ITypeSymbol symbol)
        {
            return symbol.NullableAnnotation == NullableAnnotation.Annotated && !symbol.IsNullable() ? symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) : symbol.ToDisplayString();
        }
    }
}
