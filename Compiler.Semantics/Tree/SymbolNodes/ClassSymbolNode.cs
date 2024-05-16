using System;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;

internal sealed class ClassSymbolNode : UserTypeSymbolNode, IClassSymbolNode
{
    public override IFixedList<IClassMemberDeclarationNode> Members
        => throw new NotImplementedException();

    internal ClassSymbolNode(UserTypeSymbol symbol)
        : base(symbol)
    {
        Requires.That(nameof(symbol), symbol.DeclaresType is ObjectType { IsClass: true }, "Symbol must be for an class type.");
    }
}
