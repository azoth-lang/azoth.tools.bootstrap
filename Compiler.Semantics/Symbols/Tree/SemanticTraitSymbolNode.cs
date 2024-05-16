using System;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class SemanticTraitSymbolNode : SemanticUserTypeSymbolNode, ITraitSymbolNode
{
    protected override ITraitDefinitionNode Node { get; }
    public override IFixedList<ITraitMemberDeclarationNode> Members
        => throw new NotImplementedException();

    public SemanticTraitSymbolNode(ITraitDefinitionNode node)
    {
        Node = node;
    }
}
