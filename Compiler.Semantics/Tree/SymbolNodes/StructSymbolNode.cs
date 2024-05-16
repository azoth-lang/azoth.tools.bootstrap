using System;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;

internal sealed class StructSymbolNode : UserTypeSymbolNode, IStructSymbolNode
{
    public override IFixedList<IStructMemberDeclarationNode> Members
        => throw new NotImplementedException();

    internal StructSymbolNode(UserTypeSymbol symbol)
        : base(symbol)
    {
        Requires.That(nameof(symbol), symbol.DeclaresType is StructType, "Symbol must be for a struct type.");
    }
}
