using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class WhileExpressionNode : ExpressionNode, IWhileExpressionNode
{
    public override IWhileExpressionSyntax Syntax { get; }
    private RewritableChild<IAmbiguousExpressionNode> condition;
    private bool conditionCached;
    public IAmbiguousExpressionNode TempCondition
        => GrammarAttribute.IsCached(in conditionCached) ? condition.UnsafeValue
            : this.RewritableChild(ref conditionCached, ref condition);
    public IAmbiguousExpressionNode CurrentCondition => condition.UnsafeValue;
    public IExpressionNode? Condition => TempCondition as IExpressionNode;
    private RewritableChild<IBlockExpressionNode> block;
    private bool blockCached;
    public IBlockExpressionNode Block
        => GrammarAttribute.IsCached(in blockCached) ? block.UnsafeValue
            : this.RewritableChild(ref blockCached, ref block);
    public IBlockExpressionNode CurrentBlock => block.UnsafeValue;
    private IMaybeExpressionAntetype? antetype;
    private bool antetypeCached;
    public override IMaybeExpressionAntetype Antetype
        => GrammarAttribute.IsCached(in antetypeCached) ? antetype!
            : this.Synthetic(ref antetypeCached, ref antetype,
                ExpressionAntetypesAspect.WhileExpression_Antetype);
    private DataType? type;
    private bool typeCached;
    public override DataType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type, ExpressionTypesAspect.WhileExpression_Type);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public override IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.WhileExpression_FlowStateAfter);

    public WhileExpressionNode(
        IWhileExpressionSyntax syntax,
        IAmbiguousExpressionNode condition,
        IBlockExpressionNode block)
    {
        Syntax = syntax;
        this.condition = Child.Create(this, condition);
        this.block = Child.Create(this, block);
    }

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (child == Block)
            return TempCondition.FlowLexicalScope().True;
        return base.InheritedContainingLexicalScope(child, descendant, ctx);
    }

    internal override IFlowState InheritedFlowStateBefore(
        IChildNode child,
        IChildNode descendant,
        IInheritanceContext ctx)
    {
        if (child == Block)
            return Condition?.FlowStateAfter ?? IFlowState.Empty;
        return base.InheritedFlowStateBefore(child, descendant, ctx);
    }

    protected override ControlFlowSet ComputeControlFlowNext()
        => ControlFlowAspect.WhileExpression_ControlFlowNext(this);

    internal override ControlFlowSet InheritedControlFlowFollowing(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (child == CurrentCondition)
            return ControlFlowSet.CreateNormal(Block).Union(ControlFlowFollowing());
        if (child == CurrentBlock)
            return ControlFlowSet.CreateLoop(Condition).Union(ControlFlowFollowing());
        return base.InheritedControlFlowFollowing(child, descendant, ctx);
    }

    internal override IMaybeExpressionAntetype? InheritedExpectedAntetype(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (descendant == CurrentCondition) return IAntetype.OptionalBool;
        return base.InheritedExpectedAntetype(child, descendant, ctx);
    }

    internal override DataType? InheritedExpectedType(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (descendant == CurrentCondition) return DataType.OptionalBool;
        return base.InheritedExpectedType(child, descendant, ctx);
    }
}
