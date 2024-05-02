using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

public abstract class CodeNode : SemanticNode, ICode
{
    public abstract override IConcreteSyntax Syntax { get; }
}
