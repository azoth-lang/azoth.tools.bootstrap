using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class ExpressionStatementNode : StatementNode, IExpressionStatementNode
{
    public override IExpressionStatementSyntax Syntax { get; }
    private RewritableChild<IAmbiguousExpressionNode> expression;
    private bool expressionCached;
    public IAmbiguousExpressionNode TempExpression
        => GrammarAttribute.IsCached(in expressionCached) ? expression.UnsafeValue
            : this.RewritableChild(ref expressionCached, ref expression);
    public IAmbiguousExpressionNode CurrentExpression => expression.UnsafeValue;
    public IExpressionNode? Expression => TempExpression as IExpressionNode;
    public override IMaybeAntetype? ResultAntetype => null;
    public override DataType? ResultType => null;
    public override ValueId? ResultValueId => null;
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public override IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached)
            ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.ExpressionStatement_FlowStateAfter);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public override ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.ExpressionStatement_ControlFlowNext);

    public ExpressionStatementNode(IExpressionStatementSyntax syntax, IAmbiguousExpressionNode expression)
    {
        Syntax = syntax;
        this.expression = Child.Create(this, expression);
    }

    internal override IMaybeExpressionAntetype? Inherited_ExpectedAntetype(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (descendant == CurrentExpression)
            // No expected antetype for the expression (not broadcast)
            return null;
        return base.Inherited_ExpectedAntetype(child, descendant, ctx);
    }

    internal override DataType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (descendant == CurrentExpression)
            // No expected type for the expression (not broadcast)
            return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        // By default, implicit recovery is not allowed
        => false;

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => false;
}
