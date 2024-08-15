using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class AsyncBlockExpressionSyntax : TypedExpressionSyntax, IAsyncBlockExpressionSyntax
{
    public IBlockExpressionSyntax Block { get; }

    public AsyncBlockExpressionSyntax(TextSpan span, IBlockExpressionSyntax block)
        : base(span)
    {
        Block = block;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString() => $"async {Block}";
}
