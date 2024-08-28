using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class IfExpressionNode : ExpressionNode, IIfExpressionNode
{
    public override IIfExpressionSyntax Syntax { get; }
    ICodeSyntax IElseClauseNode.Syntax => Syntax;
    private RewritableChild<IAmbiguousExpressionNode> condition;
    private bool conditionCached;
    public IAmbiguousExpressionNode TempCondition
        => GrammarAttribute.IsCached(in conditionCached) ? condition.UnsafeValue
            : this.RewritableChild(ref conditionCached, ref condition);
    public IAmbiguousExpressionNode CurrentCondition => condition.UnsafeValue;
    public IExpressionNode? Condition => TempCondition as IExpressionNode;
    private RewritableChild<IBlockOrResultNode> thenBlock;
    private bool thenBlockCached;
    public IBlockOrResultNode ThenBlock
        => GrammarAttribute.IsCached(in thenBlockCached) ? thenBlock.UnsafeValue
            : this.RewritableChild(ref thenBlockCached, ref thenBlock);
    private RewritableChild<IElseClauseNode?> elseClause;
    private bool elseClauseCached;
    public IElseClauseNode? ElseClause
        => GrammarAttribute.IsCached(in elseClauseCached) ? elseClause.UnsafeValue
            : this.RewritableChild(ref elseClauseCached, ref elseClause);
    public IElseClauseNode? CurrentElseClause => elseClause.UnsafeValue;
    private IMaybeExpressionAntetype? antetype;
    private bool antetypeCached;
    public override IMaybeExpressionAntetype Antetype
        => GrammarAttribute.IsCached(in antetypeCached) ? antetype!
            : this.Synthetic(ref antetypeCached, ref antetype,
                ExpressionAntetypesAspect.IfExpression_Antetype);
    private DataType? type;
    private bool typeCached;
    public override DataType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type, ExpressionTypesAspect.IfExpression_Type);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public override IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.IfExpression_FlowStateAfter);

    public IfExpressionNode(
        IIfExpressionSyntax syntax,
        IAmbiguousExpressionNode condition,
        IBlockOrResultNode thenBlock,
        IElseClauseNode? elseClause)
    {
        Syntax = syntax;
        this.condition = Child.Create(this, condition);
        this.thenBlock = Child.Create(this, thenBlock);
        this.elseClause = Child.Create(this, elseClause);
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (child == ThenBlock)
            return TempCondition.FlowLexicalScope().True;
        if (child == ElseClause)
            return TempCondition.FlowLexicalScope().False;
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    internal override IFlowState Inherited_FlowStateBefore(
        IChildNode child,
        IChildNode descendant,
        IInheritanceContext ctx)
    {
        if (child == ThenBlock || child == ElseClause)
            return Condition?.FlowStateAfter ?? IFlowState.Empty;
        return base.Inherited_FlowStateBefore(child, descendant, ctx);
    }

    protected override ControlFlowSet ComputeControlFlowNext()
        => ControlFlowAspect.IfExpression_ControlFlowNext(this);

    internal override ControlFlowSet Inherited_ControlFlowFollowing(
        IChildNode child,
        IChildNode descendant,
        IInheritanceContext ctx)
    {
        if (child == CurrentCondition)
        {
            if (CurrentElseClause is not null)
                return ControlFlowSet.CreateNormal(ThenBlock, ElseClause!);
            return ControlFlowSet.CreateNormal(ThenBlock).Union(ControlFlowFollowing());
        }
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
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

    protected override void CollectDiagnostics(DiagnosticCollectionBuilder diagnostics)
    {
        ExpressionTypesAspect.IfExpression_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
