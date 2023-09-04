using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class AssignmentExpression : Expression, IAssignmentExpression
{
    public IAssignableExpression LeftOperand { [DebuggerStepThrough] get; }
    public AssignmentOperator Operator { [DebuggerStepThrough] get; }
    public IExpression RightOperand { [DebuggerStepThrough] get; }

    public AssignmentExpression(
        TextSpan span,
        DataType dataType,
        ExpressionSemantics semantics,
        IAssignableExpression leftOperand,
        AssignmentOperator @operator,
        IExpression rightOperand)
        : base(span, dataType, semantics)
    {
        LeftOperand = leftOperand;
        Operator = @operator;
        RightOperand = rightOperand;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Assignment;

    public override string ToString()
        => $"{LeftOperand.ToGroupedString(ExpressionPrecedence)} {Operator.ToSymbolString()} {RightOperand.ToGroupedString(ExpressionPrecedence)}";
}
