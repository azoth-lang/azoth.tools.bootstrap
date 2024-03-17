using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class UnaryOperatorExpressionSyntax : ExpressionSyntax, IUnaryOperatorExpressionSyntax
{
    public UnaryOperatorFixity Fixity { [DebuggerStepThrough] get; }
    public UnaryOperator Operator { [DebuggerStepThrough] get; }
    public IExpressionSyntax Operand { [DebuggerStepThrough] get; }

    public UnaryOperatorExpressionSyntax(
        TextSpan span,
        UnaryOperatorFixity fixity,
        UnaryOperator @operator,
        IExpressionSyntax operand)
        : base(span)
    {
        Fixity = fixity;
        Operator = @operator;
        Operand = operand;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Unary;

    public override string ToString()
    {
        return Fixity switch
        {
            UnaryOperatorFixity.Prefix => $"{Operator.ToSymbolString()}{Operand.ToGroupedString(ExpressionPrecedence)}",
            UnaryOperatorFixity.Postfix => $"{Operand.ToGroupedString(ExpressionPrecedence)}{Operator.ToSymbolString()}",
            _ => throw ExhaustiveMatch.Failed(Fixity)
        };
    }
}
