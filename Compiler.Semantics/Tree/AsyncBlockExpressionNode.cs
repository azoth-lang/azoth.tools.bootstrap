using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class AsyncBlockExpressionNode : ExpressionNode, IAsyncBlockExpressionNode
{
    public override IAsyncBlockExpressionSyntax Syntax { get; }
    private RewritableChild<IBlockExpressionNode> block;
    private bool blockCached;
    public IBlockExpressionNode Block
        => GrammarAttribute.IsCached(in blockCached) ? block.UnsafeValue
            : this.RewritableChild(ref blockCached, ref block);
    public override IMaybeExpressionAntetype Antetype => Block.Antetype;
    public override DataType Type => Block.Type;

    public AsyncBlockExpressionNode(IAsyncBlockExpressionSyntax syntax, IBlockExpressionNode block)
    {
        Syntax = syntax;
        this.block = Child.Create(this, block);
    }
}
