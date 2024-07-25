using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace SlowestEM.Generator
{
    internal class FindClassReceiver : ISyntaxContextReceiver
    {
        public List<ClassDeclarationSyntax> ClassDeclarationSyntaxes = new List<ClassDeclarationSyntax>();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is ClassDeclarationSyntax classDeclaration)
            {
                ClassDeclarationSyntaxes.Add(classDeclaration);
            }
        }

        public IEnumerable<INamedTypeSymbol> AllClass(GeneratorExecutionContext context)
        {
            foreach (var typeNode in ClassDeclarationSyntaxes)
            {
                var symbol = context.Compilation.GetSemanticModel(typeNode.SyntaxTree)
                    .GetDeclaredSymbol(typeNode);
                if (symbol is INamedTypeSymbol namedType && !namedType.IsAbstract )
                {
                    yield return namedType;
                }
            }
        }
    }
}