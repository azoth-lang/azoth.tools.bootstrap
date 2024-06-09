using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Primitives;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;
using DotNet.Collections.Generic;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;

internal abstract class BuiltInTypeSymbolNode : ChildSymbolNode, ITypeDeclarationNode
{
    public abstract SpecialTypeName Name { get; }
    TypeName INamedDeclarationNode.Name => Name;
    public abstract override TypeSymbol Symbol { get; }
    private ValueAttribute<IFixedSet<ITypeMemberDeclarationNode>> members;
    public IFixedSet<ITypeMemberDeclarationNode> Members
        => members.TryGetValue(out var value) ? value : members.GetValue(GetMembers);
    private MultiMapHashSet<StandardName, IInstanceMemberDeclarationNode>? instanceMembersByName;
    private MultiMapHashSet<StandardName, IAssociatedMemberDeclarationNode>? associatedMembersByName;
    public IFixedSet<ITypeMemberDeclarationNode> InclusiveMembers
        // For now, the symbol tree already includes all inherited members.
        => Members;

    private protected BuiltInTypeSymbolNode()
    {
    }

    private new IFixedSet<ITypeMemberDeclarationNode> GetMembers()
        => ChildSet.Attach(this, GetMembers(Primitive.SymbolTree).OfType<ITypeMemberDeclarationNode>());

    public IEnumerable<IInstanceMemberDeclarationNode> InclusiveInstanceMembersNamed(StandardName named)
        => Members.OfType<IInstanceMemberDeclarationNode>().MembersNamed(ref instanceMembersByName, named);

    public IEnumerable<IAssociatedMemberDeclarationNode> AssociatedMembersNamed(StandardName named)
        => Members.OfType<IAssociatedMemberDeclarationNode>().MembersNamed(ref associatedMembersByName, named);
}
