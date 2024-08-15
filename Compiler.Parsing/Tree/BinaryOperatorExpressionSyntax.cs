using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class BinaryOperatorExpressionSyntax : ExpressionSyntax, IBinaryOperatorExpressionSyntax
{
    public IExpressionSyntax LeftOperand { [DebuggerStepThrough] get; }
    public BinaryOperator Operator { [DebuggerStepThrough] get; }
    public IExpressionSyntax RightOperand { [DebuggerStepThrough] get; }

    public BinaryOperatorExpressionSyntax(
        IExpressionSyntax leftOperand,
        BinaryOperator @operator,
        IExpressionSyntax rightOperand)
        : base(TextSpan.Covering(leftOperand.Span, rightOperand.Span))
    {
        LeftOperand = leftOperand;
        Operator = @operator;
        RightOperand = rightOperand;
    }

    protected override OperatorPrecedence ExpressionPrecedence => Operator.Precedence();

    public override string ToString()
        => $"{LeftOperand.ToGroupedString(ExpressionPrecedence)} {Operator.ToSymbolString()} {RightOperand.ToGroupedString(ExpressionPrecedence)}";
}
