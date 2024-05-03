using System;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class SemanticTraitSymbolNode : SemanticTypeSymbolNode, ITraitSymbolNode
{
    protected override ITraitDeclarationNode Node { get; }
    public override UserTypeSymbol Symbol => throw new NotImplementedException();
    public override IFixedList<ITraitMemberSymbolNode> Members => throw new NotImplementedException();

    public SemanticTraitSymbolNode(ITraitDeclarationNode node)
    {
        Node = node;
    }
}
