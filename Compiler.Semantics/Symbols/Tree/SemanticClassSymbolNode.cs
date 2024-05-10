using System;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class SemanticClassSymbolNode : SemanticUserTypeSymbolNode, IClassSymbolNode
{
    protected override IClassDeclarationNode Node { get; }
    public override IFixedList<IClassMemberSymbolNode> Members
        => throw new NotImplementedException();

    public SemanticClassSymbolNode(IClassDeclarationNode node)
    {
        Node = node;
    }
}
