using System;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class SemanticClassDeclarationSymbolNode : SemanticTypeDeclarationSymbolNode, IClassDeclarationSymbolNode
{
    protected override IClassDeclarationNode Node { get; }
    public override IFixedList<IClassMemberSymbolNode> Members
        => throw new NotImplementedException();

    public SemanticClassDeclarationSymbolNode(IClassDeclarationNode node)
    {
        Node = node;
    }
}
