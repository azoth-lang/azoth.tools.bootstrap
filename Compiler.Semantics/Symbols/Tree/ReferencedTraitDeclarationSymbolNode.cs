using System;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class ReferencedTraitDeclarationSymbolNode : ReferencedTypeDeclarationNode, ITraitDeclarationSymbolNode
{
    public override IFixedList<ITraitMemberSymbolNode> Members
        => throw new NotImplementedException();

    internal ReferencedTraitDeclarationSymbolNode(UserTypeSymbol symbol)
        : base(symbol)
    {
        Requires.That(nameof(symbol), symbol.DeclaresType is ObjectType { IsClass: false },
            "Symbol must be for an trait type.");
    }
}
