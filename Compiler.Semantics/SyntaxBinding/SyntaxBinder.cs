using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.SyntaxBinding;

internal static class SyntaxBinder
{
    public static Package Package(IPackageSyntax package)
        => new PackageNode(package, PackageReferences(package.References), CompilationUnits(package.CompilationUnits), CompilationUnits(package.TestingCompilationUnits));

    public static IEnumerable<PackageReference> PackageReferences(IEnumerable<IPackageReferenceSyntax> packageReferences)
        => packageReferences.Select(pr => new PackageReferenceNode(pr));

    public static IEnumerable<CompilationUnit> CompilationUnits(IEnumerable<ICompilationUnitSyntax> compilationUnits)
        => compilationUnits.Select(cu => new CompilationUnitNode(cu, UsingDirectives(cu.UsingDirectives),
            NamespaceMemberDeclarations(cu.Declarations)));

    public static IEnumerable<UsingDirective> UsingDirectives(IEnumerable<IUsingDirectiveSyntax> usingDirectives)
        => usingDirectives.Select(ud => new UsingDirectiveNode(ud));

    public static IEnumerable<NamespaceMemberDeclaration> NamespaceMemberDeclarations(IEnumerable<INonMemberDeclarationSyntax> namespaceMemberDeclarations)
        => namespaceMemberDeclarations.Select(NamespaceMemberDeclaration);

    public static NamespaceMemberDeclaration NamespaceMemberDeclaration(INonMemberDeclarationSyntax namespaceMemberDeclaration)
        => namespaceMemberDeclaration switch
        {
            INamespaceDeclarationSyntax d => NamespaceDeclaration(d),
            ITypeDeclarationSyntax d => throw new NotImplementedException(),
            IFunctionDeclarationSyntax d => throw new NotImplementedException(),
            _ => throw ExhaustiveMatch.Failed(namespaceMemberDeclaration)
        };

    private static NamespaceDeclarationNode NamespaceDeclaration(INamespaceDeclarationSyntax syntax)
        => new(syntax, UsingDirectives(syntax.UsingDirectives), NamespaceMemberDeclarations(syntax.Declarations));
}
