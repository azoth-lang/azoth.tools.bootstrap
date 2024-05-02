using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AbstractSyntax.Tree;

internal sealed class AsyncBlockExpression : Expression, IAsyncBlockExpression
{
    public IBlockExpression Block { get; }

    public AsyncBlockExpression(TextSpan span, DataType type, IBlockExpression block)
        : base(span, type)
    {
        Block = block;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString() => $"async {Block}";
}
