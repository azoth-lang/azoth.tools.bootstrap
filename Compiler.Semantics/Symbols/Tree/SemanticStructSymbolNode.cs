using System;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class SemanticStructSymbolNode : SemanticUserTypeSymbolNode, IStructSymbolNode
{
    protected override IStructDeclarationNode Node { get; }
    public override IFixedList<IStructMemberSymbolNode> Members
        => throw new NotImplementedException();

    public SemanticStructSymbolNode(IStructDeclarationNode node)
    {
        Node = node;
    }
}
