using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using System;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using Microsoft.CodeAnalysis.Operations;
using System.Diagnostics;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;

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

        internal static string GetInterceptorFilePath(this SyntaxTree? tree, Compilation compilation)
        {
            if (tree is null) return "";
            return compilation.Options.SourceReferenceResolver?.NormalizePath(tree.FilePath, baseFilePath: null) ?? tree.FilePath;
        }

        public static Location GetMemberLocation(this IInvocationOperation call)
            => GetMemberSyntax(call).GetLocation();


        public static SyntaxNode GetMemberSyntax(this IInvocationOperation call)
        {
            var syntax = call?.Syntax;
            if (syntax is null) return null!; // GIGO

            foreach (var outer in syntax.ChildNodesAndTokens())
            {
                var outerNode = outer.AsNode();
                if (outerNode is not null && outerNode is MemberAccessExpressionSyntax)
                {
                    // if there is an identifier, we want the **last** one - think Foo.Bar.Blap(...)
                    SyntaxNode? identifier = null;
                    foreach (var inner in outerNode.ChildNodesAndTokens())
                    {
                        var innerNode = inner.AsNode();
                        if (innerNode is not null && innerNode is SimpleNameSyntax)
                            identifier = innerNode;
                    }
                    // we'd prefer an identifier, but we'll allow the entire member-access
                    return identifier ?? outerNode;
                }
            }
            return syntax;
        }
    }

    public class TestData
    {
        public Location Location { get; set; }
        public string Method { get; set; }
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

        private void Generate(SourceProductionContext ctx, (Compilation Left, ImmutableArray<TestData> Right) state)
        {
            try
            {
                var s = string.Join("", state.Right.Select(i => 
                {
                    var loc = i.Location.GetLineSpan();
                    var start = loc.StartLinePosition;
                    return @$"[global::System.Runtime.CompilerServices.InterceptsLocationAttribute({SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(i.Location.SourceTree.GetInterceptorFilePath(state.Left)))},{start.Line + 1},{start.Character + 1})]
{i.Method}";
                }));
                var ss = $@"
namespace Test.AOT 
{{
    file static class GeneratedInterceptors
    {{
        {s}
    }}
}}


namespace System.Runtime.CompilerServices
{{
    // this type is needed by the compiler to implement interceptors - it doesn't need to
    // come from the runtime itself, though

    [global::System.Diagnostics.Conditional(""DEBUG"")] // not needed post-build, so: evaporate
    [global::System.AttributeUsage(global::System.AttributeTargets.Method, AllowMultiple = true)]
    sealed file class InterceptsLocationAttribute : global::System.Attribute
    {{
        public InterceptsLocationAttribute(string path, int lineNumber, int columnNumber)
        {{
            _ = path;
            _ = lineNumber;
            _ = columnNumber;
        }}
    }}
}}
";
                ctx.AddSource((state.Left.AssemblyName ?? "package") + ".generated.cs", ss);
            }
            catch (Exception ex)
            {
                //ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticsBase.UnknownError, null, ex.Message, ex.StackTrace));
            }
        }

        private TestData TransformFunc(GeneratorSyntaxContext ctx, CancellationToken token)
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
                
                var s = op.Arguments.Select(i => i.Value as IConversionOperation).Where(i => i is not null)
                    .Select(i => i.Operand as IAnonymousObjectCreationOperation)
                    .Where(i => i is not null)
                    .SelectMany(i => i.Initializers)
                    .Select(i => i as IAssignmentOperation)
                    .FirstOrDefault(i => i.Target.Type.ToDisplayString() == "string");
                return new TestData { Location = op.GetMemberLocation(), Method = @$"
internal static {op.TargetMethod.ReturnType} {op.TargetMethod.Name}_test({string.Join("", op.TargetMethod.Parameters.Select(i => @$"{i.Type} {i.Name}"))})
{{
    {(s == null ? "return null;" : $@"
    dynamic c = o;
    return c.{(s.Target as IPropertyReferenceOperation).Property.Name};
") }
}}
" };
                //var s = op.Arguments == null ? "" : "// " + string.Join("\r\n//", op.Arguments.Select(i => i.Value is IConversionOperation c ? (c.Operand is IAnonymousObjectCreationOperation o ? string.Join(",", o.Initializers.Select(i => ((i as IAssignmentOperation).Target as IPropertyReferenceOperation).Property.Name)) : c.Operand.Type?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)) : i.Value.ToString()));
                //return s + "\r\n//" + t.ToDisplayString() ;
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
