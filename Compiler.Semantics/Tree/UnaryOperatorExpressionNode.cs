using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class UnaryOperatorExpressionNode : ExpressionNode, IUnaryOperatorExpressionNode
{
    public override IUnaryOperatorExpressionSyntax Syntax { get; }
    public UnaryOperatorFixity Fixity => Syntax.Fixity;
    public UnaryOperator Operator => Syntax.Operator;
    private Child<IAmbiguousExpressionNode> operand;
    public IAmbiguousExpressionNode Operand => operand.Value;
    public IExpressionNode FinalOperand => (IExpressionNode)operand.FinalValue;
    private ValueAttribute<IMaybeExpressionAntetype> antetype;
    public override IMaybeExpressionAntetype Antetype
         => antetype.TryGetValue(out var value) ? value
             : antetype.GetValue(this, ExpressionAntetypesAspect.UnaryOperatorExpression_Antetype);
    private DataType? type;
    private bool typeCached;
    public override DataType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type, ExpressionTypesAspect.UnaryOperatorExpression_Type);
    private Circular<FlowState> flowStateAfter = new(FlowState.Empty);
    private bool flowStateAfterCached;
    public override FlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.UnaryOperatorExpression_FlowStateAfter);

    public UnaryOperatorExpressionNode(IUnaryOperatorExpressionSyntax syntax, IAmbiguousExpressionNode operand)
    {
        Syntax = syntax;
        this.operand = Child.Legacy(this, operand);
    }

    public override ConditionalLexicalScope GetFlowLexicalScope()
        => LexicalScopingAspect.UnaryOperatorExpression_GetFlowLexicalScope(this);
}
