using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class IfExpressionNode : ExpressionNode, IIfExpressionNode
{
    public override IIfExpressionSyntax Syntax { get; }
    private RewritableChild<IAmbiguousExpressionNode> condition;
    private bool conditionCached;
    public IAmbiguousExpressionNode Condition
        => GrammarAttribute.IsCached(in conditionCached) ? condition.UnsafeValue
            : this.RewritableChild(ref conditionCached, ref condition);
    public IExpressionNode? IntermediateCondition => Condition as IExpressionNode;
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
    private ValueAttribute<IMaybeExpressionAntetype> antetype;
    public override IMaybeExpressionAntetype Antetype
        => antetype.TryGetValue(out var value) ? value
            : antetype.GetValue(this, ExpressionAntetypesAspect.IfExpression_Antetype);
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

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant)
    {
        if (child == ThenBlock)
            return Condition.GetFlowLexicalScope().True;
        if (child == ElseClause)
            return Condition.GetFlowLexicalScope().False;
        return base.InheritedContainingLexicalScope(child, descendant);
    }

    internal override IFlowState InheritedFlowStateBefore(
        IChildNode child,
        IChildNode descendant,
        IInheritanceContext ctx)
    {
        if (child == ThenBlock || child == ElseClause)
            return IntermediateCondition?.FlowStateAfter ?? IFlowState.Empty;
        return base.InheritedFlowStateBefore(child, descendant, ctx);
    }
}
