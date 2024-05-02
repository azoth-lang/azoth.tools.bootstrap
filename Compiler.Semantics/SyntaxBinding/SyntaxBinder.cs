using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.SyntaxBinding;

using AST = Azoth.Tools.Bootstrap.Compiler.AST;

internal static class SyntaxBinder
{
    public static Package Package(PackageSyntax<AST.Package> package)
        => new Package(package, CompilationUnits(package.CompilationUnits), CompilationUnits(package.TestingCompilationUnits));

    public static IEnumerable<CompilationUnit> CompilationUnits(IEnumerable<ICompilationUnitSyntax> compilationUnits)
        => compilationUnits.Select(cu => new CompilationUnit(cu));
}
