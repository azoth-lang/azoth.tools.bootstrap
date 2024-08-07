using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class ResultStatementNode : StatementNode, IResultStatementNode
{
    public override IResultStatementSyntax Syntax { get; }
    private RewritableChild<IAmbiguousExpressionNode> expression;
    private bool expressionCached;
    public IAmbiguousExpressionNode Expression
        => GrammarAttribute.IsCached(in expressionCached) ? expression.UnsafeValue
            : this.RewritableChild(ref expressionCached, ref expression);
    public IAmbiguousExpressionNode CurrentExpression => expression.UnsafeValue;
    public IExpressionNode? IntermediateExpression => Expression as IExpressionNode;
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype, InheritedExpectedAntetype);
    private IMaybeAntetype? antetype;
    private bool antetypeCached;
    public IMaybeAntetype Antetype
        => GrammarAttribute.IsCached(in antetypeCached) ? antetype!
            : this.Synthetic(ref antetypeCached, ref antetype, ExpressionAntetypesAspect.ResultStatement_Antetype);
    public override IMaybeAntetype ResultAntetype => Antetype;
    private DataType? type;
    private bool typeCached;
    public DataType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type, ExpressionTypesAspect.ResultStatement_Type);
    public override DataType ResultType => Type;
    public override ValueId? ResultValueId => ValueId;
    public override IFlowState FlowStateAfter
        => IntermediateExpression?.FlowStateAfter ?? IFlowState.Empty;
    public ValueId ValueId => IntermediateExpression?.ValueId ?? default;
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public override ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.ResultStatement_ControlFlowNext);

    public ResultStatementNode(IResultStatementSyntax syntax, IAmbiguousExpressionNode expression)
    {
        Syntax = syntax;
        this.expression = Child.Create(this, expression);
    }

    public override LexicalScope GetLexicalScope()
        => InheritedContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());

    internal override IMaybeExpressionAntetype? InheritedExpectedAntetype(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (descendant == CurrentExpression)
            return ExpectedAntetype;
        return base.InheritedExpectedAntetype(child, descendant, ctx);
    }
}
