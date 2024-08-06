using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using System;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using Microsoft.CodeAnalysis.Operations;
using System.Diagnostics;
using System.Collections.Immutable;

namespace SlowestEM.Generator2
{
    public static class TypeSymbolHelper
    {
        public static ITypeSymbol? GetResultType(this IInvocationOperation invocation)
        {
            var typeArgs = invocation.TargetMethod.TypeArguments;
            if (typeArgs.Length == 1)
            {
                return typeArgs[0];
            }
            return null;
        }
    }

    [Generator(LanguageNames.CSharp)]
    public class InterceptorGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var nodes = context.SyntaxProvider.CreateSyntaxProvider(FilterFunc, TransformFunc)
                .Where(x => x is not null)
                    .Select((x, _) => x!);
            var combined = context.CompilationProvider.Combine(nodes.Collect());
            context.RegisterImplementationSourceOutput(combined, Generate);
        }

        private void Generate(SourceProductionContext ctx, (Compilation Left, ImmutableArray<object> Right) state)
        {
            try
            {
                ctx.AddSource("package" + ".generated.cs", string.Join("", state.Right.Select(i => i.ToString())));
            }
            catch (Exception ex)
            {
                //ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticsBase.UnknownError, null, ex.Message, ex.StackTrace));
            }
        }

        private object TransformFunc(GeneratorSyntaxContext ctx, CancellationToken token)
        {
            try
            {
                if (ctx.Node is not InvocationExpressionSyntax ie
                    || ctx.SemanticModel.GetOperation(ie) is not IInvocationOperation op)
                {
                    return null;
                }
                var t = op.GetResultType();
                if (t == null)
                {
                    return null;
                }
                
                var s = op.Arguments == null ? "" : "// " + string.Join("\r\n//", op.Arguments.Select(i => i.Value is IConversionOperation c ? (c.Operand is IAnonymousObjectCreationOperation o ? string.Join(",", o.Initializers.Select(i => ((i as IAssignmentOperation).Target as IPropertyReferenceOperation).Property.Name)) : c.Operand.Type?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)) : i.Value.ToString()));
                //var s = op.Arguments == null ? "" : "// " + string.Join("\r\n//", op.Arguments.Select(i => i.Value is IAnonymousObjectCreationOperation o ? " IAnonymousObjectCreationOperation: " + o.Type?.ToDisplayString() : i.Value.Type?.ToDisplayString()));
                return s + "\r\n//" + t.ToDisplayString() ;
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.Message);
                return null;
            }
        }

        private bool FilterFunc(SyntaxNode node, CancellationToken token)
        {
            if (node is InvocationExpressionSyntax ie && ie.ChildNodes().FirstOrDefault() is MemberAccessExpressionSyntax ma)
            {
                return ma.Name.ToString().StartsWith("TestInterceptor");
            }

            return false;
        }
    }
}
