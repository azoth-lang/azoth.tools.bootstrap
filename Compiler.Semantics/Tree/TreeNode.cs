using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

public abstract class TreeNode : SemanticNode
{
    public abstract ISyntax Syntax { get; }
}
