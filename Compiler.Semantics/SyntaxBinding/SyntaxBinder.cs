using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.SyntaxBinding;

internal static class SyntaxBinder
{
    public static Package Package(IPackageSyntax package)
        => new PackageNode(package, PackageReferences(package.References), CompilationUnits(package.CompilationUnits), CompilationUnits(package.TestingCompilationUnits));

    public static IEnumerable<PackageReference> PackageReferences(IEnumerable<IPackageReferenceSyntax> packageReferences)
        => packageReferences.Select(pr => new PackageReferenceNode(pr));

    public static IEnumerable<CompilationUnit> CompilationUnits(IEnumerable<ICompilationUnitSyntax> compilationUnits)
        => compilationUnits.Select(cu => new CompilationUnitNode(cu, UsingDirectives(cu.UsingDirectives)));

    public static IEnumerable<UsingDirective> UsingDirectives(IEnumerable<IUsingDirectiveSyntax> usingDirectives)
        => usingDirectives.Select(ud => new UsingDirectiveNode(ud));
}
