using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class ReturnExpressionSyntax : ExpressionSyntax, IReturnExpressionSyntax
{
    public IExpressionSyntax? Value { [DebuggerStepThrough] get; }

    public ReturnExpressionSyntax(
        TextSpan span,
        IExpressionSyntax? value)
        : base(span)
    {
        Value = value;
    }

    public override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

    public override string ToString()
        => Value is null ? "return" : $"return {Value}";
}
