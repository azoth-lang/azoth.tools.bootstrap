using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class CompilationUnit : Code
{
    public ICompilationUnitSyntax Syntax { get; }
    IConcreteSyntax Code.Syntax => Syntax;

    public CompilationUnit(ICompilationUnitSyntax syntax)
    {
        Syntax = syntax;
    }
}
