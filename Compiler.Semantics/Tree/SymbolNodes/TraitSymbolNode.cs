using System;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;

internal sealed class TraitSymbolNode : UserTypeSymbolNode, ITraitSymbolNode
{
    public override IFixedSet<ITraitMemberDeclarationNode> Members
        => throw new NotImplementedException();

    internal TraitSymbolNode(UserTypeSymbol symbol)
        : base(symbol)
    {
        Requires.That(nameof(symbol), symbol.DeclaresType is ObjectType { IsClass: false },
            "Symbol must be for an trait type.");
    }
}
