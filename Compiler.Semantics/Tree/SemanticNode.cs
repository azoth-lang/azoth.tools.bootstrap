using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

public abstract class SemanticNode : ISemanticNode
{
    public abstract ISyntax Syntax { get; }
}
