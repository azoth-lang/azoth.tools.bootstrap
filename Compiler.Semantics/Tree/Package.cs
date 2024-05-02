using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

using AST = Azoth.Tools.Bootstrap.Compiler.AST;

internal sealed class Package : Node
{
    public PackageSyntax<AST.Package> Syntax { get; }

    public IReadOnlyList<CompilationUnit> CompilationUnits { get; }
    public IReadOnlyList<CompilationUnit> TestingCompilationUnits { get; }

    public Package(PackageSyntax<AST.Package> syntax)
    {
        Syntax = syntax;
        CompilationUnits = new ChildList<CompilationUnit>(syntax.CompilationUnits.Select(cu => new CompilationUnit(cu)));
        TestingCompilationUnits = new ChildList<CompilationUnit>(syntax.TestingCompilationUnits.Select(cu => new CompilationUnit(cu)));
    }
}
