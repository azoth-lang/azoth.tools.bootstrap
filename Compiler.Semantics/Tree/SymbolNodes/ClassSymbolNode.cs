using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;

internal sealed class ClassSymbolNode : UserTypeSymbolNode, IClassSymbolNode
{
    private ValueAttribute<IFixedList<IClassMemberDeclarationNode>> members;
    public override IFixedList<IClassMemberDeclarationNode> Members
        => members.TryGetValue(out var value) ? value
            : members.GetValue(GetMembers);

    internal ClassSymbolNode(UserTypeSymbol symbol)
        : base(symbol)
    {
        Requires.That(nameof(symbol), symbol.DeclaresType is ObjectType { IsClass: true }, "Symbol must be for an class type.");
    }

    private new IFixedList<IClassMemberDeclarationNode> GetMembers()
        => ChildList.Attach(this, base.GetMembers().Cast<IClassMemberDeclarationNode>());
}
