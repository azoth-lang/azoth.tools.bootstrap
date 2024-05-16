using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class UnaryOperatorExpressionNode : ExpressionNode, IUnaryOperatorExpressionNode
{
    public override IUnaryOperatorExpressionSyntax Syntax { get; }
    public UnaryOperatorFixity Fixity => Syntax.Fixity;
    public UnaryOperator Operator => Syntax.Operator;
    private Child<IUntypedExpressionNode> operand;
    public IUntypedExpressionNode Operand => operand.Value;

    public UnaryOperatorExpressionNode(IUnaryOperatorExpressionSyntax syntax, IUntypedExpressionNode operand)
    {
        Syntax = syntax;
        this.operand = Child.Create(this, operand);
    }
}
