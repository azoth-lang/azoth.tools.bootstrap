using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal abstract class TypedExpressionSyntax : ExpressionSyntax, ITypedExpressionSyntax
{
    protected TypedExpressionSyntax(TextSpan span)
        : base(span) { }
}
