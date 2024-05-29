using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class LoopExpressionNode : ExpressionNode, ILoopExpressionNode
{
    public override ILoopExpressionSyntax Syntax { get; }
    public IBlockExpressionNode Block { get; }

    public LoopExpressionNode(ILoopExpressionSyntax syntax, IBlockExpressionNode block)
    {
        Syntax = syntax;
        Block = Child.Attach(this, block);
    }

    internal override IFlowNode InheritedPredecessor(IChildNode child, IChildNode descendant)
    {
        if (descendant == Block)
            return Predecessor();
        return base.InheritedPredecessor(child, descendant);
    }
}
