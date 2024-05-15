using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class UnaryOperatorExpressionNode : ExpressionNode, IUnaryOperatorExpressionNode
{
    public override IUnaryOperatorExpressionSyntax Syntax { get; }
    public UnaryOperatorFixity Fixity => Syntax.Fixity;
    public UnaryOperator Operator => Syntax.Operator;
    public IUntypedExpressionNode Operand { get; }

    public UnaryOperatorExpressionNode(IUnaryOperatorExpressionSyntax syntax, IUntypedExpressionNode operand)
    {
        Syntax = syntax;
        Operand = Child.Attach(this, operand);
    }
}
