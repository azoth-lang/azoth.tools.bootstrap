using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

public abstract class CodeNode : TreeNode, Code
{
    public abstract override IConcreteSyntax Syntax { get; }
}
