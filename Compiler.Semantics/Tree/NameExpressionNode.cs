using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class NameExpressionNode : CodeNode, INameExpressionNode
{
    public abstract override INameExpressionSyntax Syntax { get; }
    IExpressionSyntax IUntypedExpressionNode.Syntax => Syntax;

    private protected NameExpressionNode() { }
}
