using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal abstract class DataTypedExpressionSyntax : TypedExpressionSyntax, IDataTypedExpressionSyntax
{
    public override Promise<DataType> DataType { get; } = new();

    protected DataTypedExpressionSyntax(TextSpan span)
        : base(span) { }
}
