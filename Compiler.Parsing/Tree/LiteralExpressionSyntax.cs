using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal abstract class LiteralExpressionSyntax : TypedExpressionSyntax, ILiteralExpressionSyntax
{
    protected LiteralExpressionSyntax(TextSpan span)
        : base(span)
    {
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;
}
