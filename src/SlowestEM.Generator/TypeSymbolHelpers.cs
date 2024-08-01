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

        internal static IEnumerable<IPropertySymbol> GetAllGettableProperties(this ITypeSymbol typeSymbol)
        {
            var result = typeSymbol
                .GetMembers()
                .Where(s => s.Kind == SymbolKind.Property).Cast<IPropertySymbol>()
                .Where(p => p.GetMethod?.DeclaredAccessibility == Accessibility.Public)
                .Union(typeSymbol.BaseType == null ? new IPropertySymbol[0] : typeSymbol.BaseType.GetAllGettableProperties());

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

        internal static bool IsEnum(this ITypeSymbol symbol)
        {
            return symbol.TypeKind == TypeKind.Enum || (symbol.IsNullable() && symbol is INamedTypeSymbol namedType  && namedType.TypeArguments[0].TypeKind == TypeKind.Enum);
        }

        internal static INamedTypeSymbol GetEnumUnderlyingType(this ITypeSymbol symbol)
        {
            if (symbol is INamedTypeSymbol namedType)
            {
                if (symbol.TypeKind == TypeKind.Enum)
                    return namedType.EnumUnderlyingType;
                else if (symbol.IsNullable() && namedType.TypeArguments[0].TypeKind == TypeKind.Enum && namedType.TypeArguments[0] is INamedTypeSymbol tn)
                {
                    return tn.EnumUnderlyingType;
                }
            }
            return null;
        }

        internal static string ToNoNullableDisplayString(this ITypeSymbol symbol)
        {
            return symbol.IsNullable() && symbol is INamedTypeSymbol pnt ? pnt.TypeArguments[0].ToRealTypeDisplayString() : symbol.ToRealTypeDisplayString();
        }

        internal static bool HasAttribute(this IPropertySymbol symbol, string attribute)
        {
            return symbol.GetAttributes().Any(i => i.AttributeClass.Name == attribute);
        }
    }
}
