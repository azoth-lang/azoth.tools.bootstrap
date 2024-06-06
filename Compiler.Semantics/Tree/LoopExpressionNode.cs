using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class LoopExpressionNode : ExpressionNode, ILoopExpressionNode
{
    public override ILoopExpressionSyntax Syntax { get; }
    public IBlockExpressionNode Block { get; }
    private ValueAttribute<IMaybeExpressionAntetype> antetype;
    public override IMaybeExpressionAntetype Antetype
        => antetype.TryGetValue(out var value) ? value
            : antetype.GetValue(this, ExpressionAntetypesAspect.LoopExpression_Antetype);

    public LoopExpressionNode(ILoopExpressionSyntax syntax, IBlockExpressionNode block)
    {
        Syntax = syntax;
        Block = Child.Attach(this, block);
    }
}
