using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
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
    }
}