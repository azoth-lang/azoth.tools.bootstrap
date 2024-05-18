using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class ExpressionNode : AmbiguousExpressionNode, IExpressionNode
{
    public abstract override ITypedExpressionSyntax Syntax { get; }

    private protected ExpressionNode() { }
}
