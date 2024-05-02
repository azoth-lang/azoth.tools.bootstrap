using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics;

[GeneratedCode("AzothCompilerCodeGen", null)]
public static class ISemanticNodeExtensions
{
    [DebuggerStepThrough]
    public static IEnumerable<ISemanticNode> Children(this ISemanticNode node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case IPackage n:
                foreach (var child in n.References)
                    yield return child;
                foreach (var child in n.CompilationUnits)
                    yield return child;
                foreach (var child in n.TestingCompilationUnits)
                    yield return child;
                yield break;
            case IPackageReference n:
                yield break;
            case ICompilationUnit n:
                foreach (var child in n.UsingDirectives)
                    yield return child;
                foreach (var child in n.Declarations)
                    yield return child;
                yield break;
            case IUsingDirective n:
                yield break;
            case INamespaceDeclaration n:
                foreach (var child in n.UsingDirectives)
                    yield return child;
                foreach (var child in n.Declarations)
                    yield return child;
                yield break;
            case IClassDeclaration n:
                foreach (var child in n.GenericParameters)
                    yield return child;
                if (n.BaseTypeName is not null)
                    yield return n.BaseTypeName;
                foreach (var child in n.SupertypeNames)
                    yield return child;
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case IStructDeclaration n:
                foreach (var child in n.GenericParameters)
                    yield return child;
                foreach (var child in n.SupertypeNames)
                    yield return child;
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case ITraitDeclaration n:
                foreach (var child in n.GenericParameters)
                    yield return child;
                foreach (var child in n.SupertypeNames)
                    yield return child;
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case IGenericParameter n:
                yield return n.Constraint;
                yield break;
            case IUnresolvedSupertypeName n:
                yield break;
            case ICapabilitySet n:
                yield break;
            case ICapability n:
                yield break;
        }
    }
}
