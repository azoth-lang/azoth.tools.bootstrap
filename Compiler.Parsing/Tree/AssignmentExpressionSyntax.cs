using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal sealed class AssignmentExpressionSyntax : TypedExpressionSyntax, IAssignmentExpressionSyntax
{
    public IAssignableExpressionSyntax LeftOperand { [DebuggerStepThrough] get; }
    public AssignmentOperator Operator { [DebuggerStepThrough] get; }
    public IExpressionSyntax RightOperand { [DebuggerStepThrough] get; }

    public AssignmentExpressionSyntax(
        IAssignableExpressionSyntax leftOperand,
        AssignmentOperator @operator,
        IExpressionSyntax rightOperand)
        : base(TextSpan.Covering(leftOperand.Span, rightOperand.Span))
    {
        LeftOperand = leftOperand;
        Operator = @operator;
        RightOperand = rightOperand;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Assignment;

    public override string ToString()
        => $"{LeftOperand.ToGroupedString(ExpressionPrecedence)} {Operator.ToSymbolString()} {RightOperand.ToGroupedString(ExpressionPrecedence)}";
}
