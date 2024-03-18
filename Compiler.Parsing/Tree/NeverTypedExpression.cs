using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal abstract class NeverTypedExpression : TypedExpressionSyntax, INeverTypedExpressionSyntax
{
    public override Promise<NeverType> DataType => Types.DataType.PromiseOfNever;

    protected NeverTypedExpression(TextSpan span)
        : base(span) { }
}
