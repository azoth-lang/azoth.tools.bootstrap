using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal abstract class DataTypedExpressionSyntax : TypedExpressionSyntax, IDataTypedExpressionSyntax
{
    protected DataTypedExpressionSyntax(TextSpan span)
        : base(span) { }
}
