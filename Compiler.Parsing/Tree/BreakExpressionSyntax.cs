using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class BreakExpressionSyntax : NeverTypedExpression, IBreakExpressionSyntax
{
    public IExpressionSyntax? Value { [DebuggerStepThrough] get; }

    public BreakExpressionSyntax(
        TextSpan span,
        IExpressionSyntax? value)
        : base(span)
    {
        Value = value;
    }

    protected override OperatorPrecedence ExpressionPrecedence => Value is not null ? OperatorPrecedence.Min : OperatorPrecedence.Primary;

    public override string ToString()
    {
        if (Value is not null)
            return $"break {Value}";
        return "break";
    }
}
