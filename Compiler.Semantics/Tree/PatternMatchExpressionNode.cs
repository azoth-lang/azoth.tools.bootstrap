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

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (child == Pattern)
            return TempReferent.FlowLexicalScope().True;
        return ContainingLexicalScope;
    }

    public override ConditionalLexicalScope FlowLexicalScope() => Pattern.FlowLexicalScope();

    internal override IMaybeAntetype InheritedContextBindingAntetype(IChildNode child, IChildNode descendant)
    {
        if (descendant == Pattern)
            return NameBindingAntetypesAspect.PatternMatchExpression_Pattern_ContextBindingAntetype_(this);
        return base.InheritedContextBindingAntetype(child, descendant);
    }

    internal override DataType InheritedContextBindingType(IChildNode child, IChildNode descendant)
    {
        if (descendant == Pattern)
            return NameBindingTypesAspect.PatternMatchExpression_Pattern_ContextBindingType(this);
        return base.InheritedContextBindingType(child, descendant);
    }

    internal override IFlowState InheritedFlowStateBefore(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (child == Pattern)
            return Referent?.FlowStateAfter ?? IFlowState.Empty;
        return base.InheritedFlowStateBefore(child, descendant, ctx);
    }

    internal override ValueId? InheritedMatchReferentValueId(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (child == Pattern)
            return Referent?.ValueId;
        return base.InheritedMatchReferentValueId(child, descendant, ctx);
    }

    protected override ControlFlowSet ComputeControlFlowNext()
        => ControlFlowAspect.PatternMatchExpression_ControlFlowNext(this);

    internal override ControlFlowSet InheritedControlFlowFollowing(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (child == CurrentReferent) return ControlFlowSet.CreateNormal(Pattern);
        return base.InheritedControlFlowFollowing(child, descendant, ctx);
    }
}
