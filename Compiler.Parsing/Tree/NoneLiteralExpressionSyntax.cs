using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class NoneLiteralExpressionSyntax : LiteralExpressionSyntax, INoneLiteralExpressionSyntax
{
    public NoneLiteralExpressionSyntax(TextSpan span)
        : base(span)
    {
    }

    public override string ToString() => "none";
}
