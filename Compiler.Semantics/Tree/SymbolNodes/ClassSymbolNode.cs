using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;

internal sealed class ClassSymbolNode : UserTypeSymbolNode, IClassSymbolNode
{
    private ValueAttribute<IFixedSet<IClassMemberSymbolNode>> members;
    public override IFixedSet<IClassMemberSymbolNode> Members
        => members.TryGetValue(out var value) ? value
            : members.GetValue(GetMembers);
    public override IFixedSet<IClassMemberDeclarationNode> InclusiveMembers
        // For now, the symbol tree already includes all inherited members.
        => Members;

    internal ClassSymbolNode(UserTypeSymbol symbol)
        : base(symbol)
    {
        Requires.That(symbol.DeclaresType is ObjectType { IsClass: true }, nameof(symbol), "Symbol must be for an class type.");
    }

    private new IFixedSet<IClassMemberSymbolNode> GetMembers()
        => ChildSet.Attach(this, base.GetMembers().OfType<IClassMemberSymbolNode>());
}
