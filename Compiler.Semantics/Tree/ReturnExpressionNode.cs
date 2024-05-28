using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class ReturnExpressionNode : ExpressionNode, IReturnExpressionNode
{
    public override IReturnExpressionSyntax Syntax { get; }
    public override NeverType Type => DataType.Never;
    private Child<IAmbiguousExpressionNode?> value;
    public IAmbiguousExpressionNode? Value => value.Value;

    public ReturnExpressionNode(IReturnExpressionSyntax syntax, IAmbiguousExpressionNode? value)
    {
        Syntax = syntax;
        this.value = Child.Create(this, value);
    }

    internal override ISemanticNode? InheritedPredecessor(IChildNode child, IChildNode descendant)
    {
        if (descendant == Value) return Predecessor();
        return base.InheritedPredecessor(child, descendant);
    }
}
