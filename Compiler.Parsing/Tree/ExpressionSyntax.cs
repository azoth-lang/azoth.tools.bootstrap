using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal abstract class ExpressionSyntax : CodeSyntax, IExpressionSyntax
{
    protected ExpressionSyntax(TextSpan span)
        : base(span)
    {
    }

    protected abstract OperatorPrecedence ExpressionPrecedence { get; }

    public string ToGroupedString(OperatorPrecedence surroundingPrecedence)
        => surroundingPrecedence > ExpressionPrecedence ? $"({this})" : ToString();
}
