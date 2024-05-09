using System;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class ReferencedTraitSymbolNode : ReferencedTypeNode, ITraitSymbolNode
{
    public override IFixedList<ITraitMemberSymbolNode> Members
        => throw new NotImplementedException();

    internal ReferencedTraitSymbolNode(UserTypeSymbol symbol)
        : base(symbol)
    {
        Requires.That(nameof(symbol), symbol.DeclaresType is ObjectType { IsClass: false },
            "Symbol must be for an trait type.");
    }
}