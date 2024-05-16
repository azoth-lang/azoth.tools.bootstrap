using System;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class SemanticStructSymbolNode : SemanticUserTypeSymbolNode, IStructDeclarationNode
{
    protected override IStructDefinitionNode Node { get; }
    public override IFixedList<IStructMemberDeclarationNode> Members
        => throw new NotImplementedException();

    public SemanticStructSymbolNode(IStructDefinitionNode node)
    {
        Node = node;
    }
}
