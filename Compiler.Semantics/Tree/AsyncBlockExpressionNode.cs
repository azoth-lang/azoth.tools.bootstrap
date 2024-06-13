using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class AsyncBlockExpressionNode : ExpressionNode, IAsyncBlockExpressionNode
{
    public override IAsyncBlockExpressionSyntax Syntax { get; }
    public IBlockExpressionNode Block { get; }
    public override IMaybeExpressionAntetype Antetype => Block.Antetype;
    public override DataType Type => Block.Type;

    public AsyncBlockExpressionNode(IAsyncBlockExpressionSyntax syntax, IBlockExpressionNode block)
    {
        Syntax = syntax;
        Block = Child.Attach(this, block);
    }
}
