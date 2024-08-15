using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;

internal sealed class TraitSymbolNode : UserTypeSymbolNode, ITraitSymbolNode
{
    private ValueAttribute<IFixedSet<ITraitMemberDeclarationNode>> members;
    public override IFixedSet<ITraitMemberDeclarationNode> Members
        => members.TryGetValue(out var value) ? value
            : members.GetValue(GetMembers);
    public override IFixedSet<ITraitMemberDeclarationNode> InclusiveMembers
        // For now, the symbol tree already includes all inherited members.
        => Members;

    internal TraitSymbolNode(UserTypeSymbol symbol)
        : base(symbol)
    {
        Requires.That(symbol.DeclaresType is ObjectType { IsClass: false }, nameof(symbol),
            "Symbol must be for an trait type.");
    }

    private new IFixedSet<ITraitMemberDeclarationNode> GetMembers()
        => ChildSet.Attach(this, base.GetMembers().OfType<ITraitMemberDeclarationNode>());
}
