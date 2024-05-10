using System;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class ReferencedStructDeclarationSymbolNode : ReferencedTypeDeclarationNode, IStructDeclarationSymbolNode
{
    public override IFixedList<IStructMemberSymbolNode> Members
        => throw new NotImplementedException();

    internal ReferencedStructDeclarationSymbolNode(UserTypeSymbol symbol)
        : base(symbol)
    {
        Requires.That(nameof(symbol), symbol.DeclaresType is StructType, "Symbol must be for a struct type.");
    }
}
