using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class NoneLiteralExpressionSyntax : LiteralExpressionSyntax, INoneLiteralExpressionSyntax
{
    public NoneLiteralExpressionSyntax(TextSpan span)
        : base(span, ExpressionSemantics.CopyValue)
    {
    }

    public override string ToString()
    {
        return "none";
    }
}
