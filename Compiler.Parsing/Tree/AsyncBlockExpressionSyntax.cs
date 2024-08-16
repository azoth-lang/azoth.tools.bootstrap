using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class AsyncBlockExpressionSyntax : ExpressionSyntax, IAsyncBlockExpressionSyntax
{
    public IBlockExpressionSyntax Block { get; }

    public AsyncBlockExpressionSyntax(TextSpan span, IBlockExpressionSyntax block)
        : base(span)
    {
        Block = block;
    }

    public override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString() => $"async {Block}";
}
