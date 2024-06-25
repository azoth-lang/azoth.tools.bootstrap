using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class PatternMatchExpressionNode : ExpressionNode, IPatternMatchExpressionNode
{
    public override IPatternMatchExpressionSyntax Syntax { get; }
    private RewritableChild<IAmbiguousExpressionNode> referent;
    private bool referentCached;
    public IAmbiguousExpressionNode Referent
        => GrammarAttribute.IsCached(in referentCached) ? referent.UnsafeValue
            : this.RewritableChild(ref referentCached, ref referent);
    public IExpressionNode? IntermediateReferent => Referent as IExpressionNode;
    public IPatternNode Pattern { get; }
    public override IMaybeExpressionAntetype Antetype => IAntetype.Bool;
    public override DataType Type => DataType.Bool;
    public override FlowState FlowStateAfter => Pattern.FlowStateAfter;

    public PatternMatchExpressionNode(
        IPatternMatchExpressionSyntax syntax,
        IAmbiguousExpressionNode referent,
        IPatternNode pattern)
    {
        Syntax = syntax;
        this.referent = Child.Create(this, referent);
        Pattern = Child.Attach(this, pattern);
    }

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant)
    {
        if (child == Pattern)
            return Referent.GetFlowLexicalScope().True;
        return GetContainingLexicalScope();
    }

    public override ConditionalLexicalScope GetFlowLexicalScope() => Pattern.GetFlowLexicalScope();

    internal override IMaybeAntetype InheritedBindingAntetype(IChildNode child, IChildNode descendant)
    {
        if (descendant == Pattern)
            return NameBindingAntetypesAspect.PatternMatchExpression_InheritedBindingAntetype_Pattern(this);
        return base.InheritedBindingAntetype(child, descendant);
    }

    internal override DataType InheritedBindingType(IChildNode child, IChildNode descendant)
    {
        if (descendant == Pattern)
            return NameBindingTypesAspect.PatternMatchExpression_InheritedBindingType_Pattern(this);
        return base.InheritedBindingType(child, descendant);
    }

    internal override FlowState InheritedFlowStateBefore(
        IChildNode child,
        IChildNode descendant,
        IInheritanceContext ctx)
    {
        if (child == Pattern)
            return IntermediateReferent?.FlowStateAfter ?? FlowState.Empty;
        return base.InheritedFlowStateBefore(child, descendant, ctx);
    }
}
