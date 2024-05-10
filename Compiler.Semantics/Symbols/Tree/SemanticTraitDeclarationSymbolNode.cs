using System;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class SemanticTraitDeclarationSymbolNode : SemanticTypeDeclarationSymbolNode, ITraitDeclarationSymbolNode
{
    protected override ITraitDeclarationNode Node { get; }
    public override IFixedList<ITraitMemberSymbolNode> Members
        => throw new NotImplementedException();

    public SemanticTraitDeclarationSymbolNode(ITraitDeclarationNode node)
    {
        Node = node;
    }
}
