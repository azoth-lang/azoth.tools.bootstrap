using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class NoneLiteralExpressionSyntax : LiteralExpressionSyntax, INoneLiteralExpressionSyntax
{
    public override Promise<OptionalType> DataType => Types.DataType.PromiseOfNone;

    public NoneLiteralExpressionSyntax(TextSpan span)
        : base(span)
    {
    }

    public override string ToString() => "none";
}
