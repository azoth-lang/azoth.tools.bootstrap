using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal abstract class NeverTypedExpression : TypedExpressionSyntax, INeverTypedExpressionSyntax
{
    protected NeverTypedExpression(TextSpan span)
        : base(span) { }
}
