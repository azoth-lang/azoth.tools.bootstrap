using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;

internal sealed class StructSymbolNode : UserTypeSymbolNode, IStructSymbolNode
{
    private ValueAttribute<IFixedSet<IStructMemberSymbolNode>> members;
    public override IFixedSet<IStructMemberSymbolNode> Members
        => members.TryGetValue(out var value) ? value
            : members.GetValue(GetMembers);
    public override IFixedSet<IStructMemberDeclarationNode> InclusiveMembers
        // For now, the symbol tree already includes all inherited members.
        => Members;

    internal StructSymbolNode(UserTypeSymbol symbol)
        : base(symbol)
    {
        Requires.That(symbol.DeclaresType is StructType, nameof(symbol), "Symbol must be for a struct type.");
    }

    private new IFixedSet<IStructMemberSymbolNode> GetMembers()
        => ChildSet.Attach(this, base.GetMembers().OfType<IStructMemberSymbolNode>());
}
