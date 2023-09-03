using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class BreakExpressionSyntax : ExpressionSyntax, IBreakExpressionSyntax
{
    public IExpressionSyntax? Value { [DebuggerStepThrough] get; }

    public BreakExpressionSyntax(
        TextSpan span,
        IExpressionSyntax? value)
        : base(span, ExpressionSemantics.Void)
    {
        Value = value;
    }

    protected override OperatorPrecedence ExpressionPrecedence => Value != null ? OperatorPrecedence.Min : OperatorPrecedence.Primary;

    public override string ToString()
    {
        if (Value != null)
            return $"break {Value}";
        return "break";
    }
}
