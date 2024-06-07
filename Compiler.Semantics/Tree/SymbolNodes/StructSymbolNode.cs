using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;

internal sealed class StructSymbolNode : UserTypeSymbolNode, IStructSymbolNode
{
    private ValueAttribute<IFixedSet<IStructMemberDeclarationNode>> members;
    public override IFixedSet<IStructMemberDeclarationNode> Members
        => members.TryGetValue(out var value) ? value
            : members.GetValue(GetMembers);

    internal StructSymbolNode(UserTypeSymbol symbol)
        : base(symbol)
    {
        Requires.That(nameof(symbol), symbol.DeclaresType is StructType, "Symbol must be for a struct type.");
    }

    private new IFixedSet<IStructMemberDeclarationNode> GetMembers()
        => ChildSet.Attach(this, base.GetMembers().OfType<IStructMemberDeclarationNode>());
}
