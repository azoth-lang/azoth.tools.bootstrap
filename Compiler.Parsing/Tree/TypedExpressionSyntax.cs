using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal abstract class TypedExpressionSyntax : ExpressionSyntax, ITypedExpressionSyntax
{
    public abstract override IPromise<DataType> DataType { get; }

    protected TypedExpressionSyntax(TextSpan span)
        : base(span) { }
}
