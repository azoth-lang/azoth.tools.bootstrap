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

internal sealed class PatternMatchExpressionNode : ExpressionNode, IPatternMatchExpressionNode
{
    public override IPatternMatchExpressionSyntax Syntax { get; }
    private RewritableChild<IAmbiguousExpressionNode> referent;
    private bool referentCached;
    public IAmbiguousExpressionNode TempReferent
        => GrammarAttribute.IsCached(in referentCached) ? referent.UnsafeValue
            : this.RewritableChild(ref referentCached, ref referent);
    public IAmbiguousExpressionNode CurrentReferent => referent.UnsafeValue;
    public IExpressionNode? Referent => TempReferent as IExpressionNode;
    public IPatternNode Pattern { get; }
    public override IMaybeExpressionAntetype Antetype => IAntetype.Bool;
    public override DataType Type => DataType.Bool;
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public override IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached)
            ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.PatternMatchExpression_FlowStateAfter);

    public PatternMatchExpressionNode(
        IPatternMatchExpressionSyntax syntax,
        IAmbiguousExpressionNode referent,
        IPatternNode pattern)
    {
        Syntax = syntax;
        this.referent = Child.Create(this, referent);
        Pattern = Child.Attach(this, pattern);
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (child == Pattern)
            return TempReferent.FlowLexicalScope().True;
        return ContainingLexicalScope;
    }

    public override ConditionalLexicalScope FlowLexicalScope() => Pattern.FlowLexicalScope();

    internal override IMaybeAntetype Inherited_ContextBindingAntetype(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (descendant == Pattern)
            return NameBindingAntetypesAspect.PatternMatchExpression_Pattern_ContextBindingAntetype_(this);
        return base.Inherited_ContextBindingAntetype(child, descendant, ctx);
    }

    internal override DataType Inherited_ContextBindingType(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (descendant == Pattern)
            return NameBindingTypesAspect.PatternMatchExpression_Pattern_ContextBindingType(this);
        return base.Inherited_ContextBindingType(child, descendant, ctx);
    }

    internal override IFlowState Inherited_FlowStateBefore(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (child == Pattern)
            return Referent?.FlowStateAfter ?? IFlowState.Empty;
        return base.Inherited_FlowStateBefore(child, descendant, ctx);
    }

    internal override ValueId? Inherited_MatchReferentValueId(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (child == Pattern)
            return Referent?.ValueId;
        return base.Inherited_MatchReferentValueId(child, descendant, ctx);
    }

    protected override ControlFlowSet ComputeControlFlowNext()
        => ControlFlowAspect.PatternMatchExpression_ControlFlowNext(this);

    internal override ControlFlowSet Inherited_ControlFlowFollowing(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (child == CurrentReferent) return ControlFlowSet.CreateNormal(Pattern);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }
}
