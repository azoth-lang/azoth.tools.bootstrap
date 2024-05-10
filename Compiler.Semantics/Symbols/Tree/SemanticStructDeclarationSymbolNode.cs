using System;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class SemanticStructDeclarationSymbolNode : SemanticTypeDeclarationSymbolNode, IStructDeclarationSymbolNode
{
    protected override IStructDeclarationNode Node { get; }
    public override IFixedList<IStructMemberSymbolNode> Members
        => throw new NotImplementedException();

    public SemanticStructDeclarationSymbolNode(IStructDeclarationNode node)
    {
        Node = node;
    }
}
