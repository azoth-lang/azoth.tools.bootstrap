using System;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class SemanticTraitSymbolNode : SemanticUserTypeSymbolNode, ITraitSymbolNode
{
    protected override ITraitDeclarationNode Node { get; }
    public override IFixedList<ITraitMemberSymbolNode> Members
        => throw new NotImplementedException();

    public SemanticTraitSymbolNode(ITraitDeclarationNode node)
    {
        Node = node;
    }
}
