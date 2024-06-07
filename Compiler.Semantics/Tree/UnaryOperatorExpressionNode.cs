using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

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

    public UnaryOperatorExpressionNode(IUnaryOperatorExpressionSyntax syntax, IAmbiguousExpressionNode operand)
    {
        Syntax = syntax;
        this.operand = Child.Create(this, operand);
    }

    public override ConditionalLexicalScope GetFlowLexicalScope()
        => LexicalScopingAspect.UnaryOperatorExpression_GetFlowLexicalScope(this);
}
