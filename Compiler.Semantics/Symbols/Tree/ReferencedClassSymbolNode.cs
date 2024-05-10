using System;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class ReferencedClassSymbolNode : ReferencedUserTypeSymbolNode, IClassSymbolNode
{
    public override IFixedList<IClassMemberSymbolNode> Members
        => throw new NotImplementedException();

    internal ReferencedClassSymbolNode(UserTypeSymbol symbol)
        : base(symbol)
    {
        Requires.That(nameof(symbol), symbol.DeclaresType is ObjectType { IsClass: true }, "Symbol must be for an class type.");
    }
}
