using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal sealed class StringLiteralExpressionSyntax : LiteralExpressionSyntax, IStringLiteralExpressionSyntax
{
    public string Value { [DebuggerStepThrough] get; }

    public StringLiteralExpressionSyntax(TextSpan span, string value)
        : base(span)
    {
        Value = value;
    }

    public override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString() => $"\"{Value.Escape()}\"";
}
