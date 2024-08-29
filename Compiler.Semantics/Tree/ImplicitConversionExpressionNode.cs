using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal class ImplicitConversionExpressionNode : ExpressionNode, IImplicitConversionExpressionNode
{
    public override IExpressionSyntax Syntax => (IExpressionSyntax)Referent.Syntax;
    private RewritableChild<IExpressionNode> referent;
    private bool referentCached;
    public IExpressionNode Referent
        => GrammarAttribute.IsCached(in referentCached) ? referent.UnsafeValue
            : this.RewritableChild(ref referentCached, ref referent);
    public IExpressionNode CurrentReferent => referent.UnsafeValue;
    public override SimpleAntetype Antetype { get; }
    public override DataType Type { get; }
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public override IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.ImplicitConversionExpression_FlowStateAfter);

    public ImplicitConversionExpressionNode(
        IExpressionNode referent,
        SimpleAntetype convertToAntetype)
    {
        this.referent = Child.Create(this, referent);
        Antetype = convertToAntetype;
        Type = convertToAntetype.ToType();
    }

    public override ConditionalLexicalScope FlowLexicalScope() => Referent.FlowLexicalScope();

    internal override IMaybeExpressionAntetype? Inherited_ExpectedAntetype(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (descendant == CurrentReferent)
            // No expected antetype for the referent. If one were given, it could cause another implicit conversion.
            return null;
        return base.Inherited_ExpectedAntetype(child, descendant, ctx);
    }

    internal override DataType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (descendant == CurrentReferent)
            // No expected antetype for the referent. If one were given, it could cause another implicit conversion.
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    protected override ControlFlowSet ComputeControlFlowNext()
        => ControlFlowAspect.ImplicitConversionExpression_ControlFlowNext(this);
}
