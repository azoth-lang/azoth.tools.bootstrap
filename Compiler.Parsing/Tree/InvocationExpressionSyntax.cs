using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class InvocationExpressionSyntax : TypedExpressionSyntax, IInvocationExpressionSyntax
{
    public IExpressionSyntax Expression { [DebuggerStepThrough] get; }
    public IFixedList<IExpressionSyntax> Arguments { [DebuggerStepThrough] get; }

    public InvocationExpressionSyntax(
        TextSpan span,
        IExpressionSyntax expression,
        IFixedList<IExpressionSyntax> arguments)
        : base(span)
    {
        Expression = expression;
        Arguments = arguments;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString() => $"{Expression}({string.Join(", ", Arguments)})";
}
