using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

public abstract class Declaration : Code
{
    public abstract IConcreteSyntax Syntax { get; }
}
