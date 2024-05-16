using System;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class ReferencedStructSymbolNode : ReferencedUserTypeSymbolNode, IStructDeclarationNode
{
    public override IFixedList<IStructMemberDeclarationNode> Members
        => throw new NotImplementedException();

    internal ReferencedStructSymbolNode(UserTypeSymbol symbol)
        : base(symbol)
    {
        Requires.That(nameof(symbol), symbol.DeclaresType is StructType, "Symbol must be for a struct type.");
    }
}
