using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class BlockBodyNode : CodeNode, IBlockBodyNode
{
    public override IBlockBodySyntax Syntax { get; }

    public BlockBodyNode(IBlockBodySyntax syntax)
    {
        Syntax = syntax;
    }
}
