using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class StringLiteralExpressionSyntax : LiteralExpressionSyntax, IStringLiteralExpressionSyntax
{
    public string Value { [DebuggerStepThrough] get; }

    public StringLiteralExpressionSyntax(TextSpan span, string value)
        : base(span)
    {
        Value = value;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString()
    {
        return $"\"{Value.Escape()}\"";
    }
}
