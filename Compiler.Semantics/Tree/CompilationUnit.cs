using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class CompilationUnit : Node
{
    public ICompilationUnitSyntax Syntax { get; }

    public CompilationUnit(ICompilationUnitSyntax syntax)
    {
        Syntax = syntax;
    }
}
