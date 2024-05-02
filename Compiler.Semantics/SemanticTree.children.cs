using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics;

[GeneratedCode("AzothCompilerCodeGen", null)]
public static class SemanticNodeExtensions
{
    [DebuggerStepThrough]
    public static IEnumerable<SemanticNode> Children(this SemanticNode node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case Package n:
                foreach (var child in n.References)
                    yield return child;
                foreach (var child in n.CompilationUnits)
                    yield return child;
                foreach (var child in n.TestingCompilationUnits)
                    yield return child;
                yield break;
            case PackageReference n:
                yield break;
            case CompilationUnit n:
                foreach (var child in n.UsingDirectives)
                    yield return child;
                foreach (var child in n.Declarations)
                    yield return child;
                yield break;
            case NamespaceDeclaration n:
                foreach (var child in n.UsingDirectives)
                    yield return child;
                foreach (var child in n.Declarations)
                    yield return child;
                yield break;
            case UsingDirective n:
                yield break;
        }
    }
}
