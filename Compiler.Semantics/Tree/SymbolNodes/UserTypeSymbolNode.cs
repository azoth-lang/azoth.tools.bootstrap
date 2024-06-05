using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;
using DotNet.Collections.Generic;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;

internal abstract class UserTypeSymbolNode : PackageFacetChildSymbolNode, IUserTypeDeclarationNode
{
    public override StandardName Name => base.Name!;

    public override UserTypeSymbol Symbol { get; }
    public abstract IFixedSet<ITypeMemberDeclarationNode> Members { get; }
    private MultiMapHashSet<StandardName, IInstanceMemberDeclarationNode>? instanceMembersByName;
    private MultiMapHashSet<StandardName, IAssociatedMemberDeclarationNode>? associatedMembersByName;

    private protected UserTypeSymbolNode(UserTypeSymbol symbol)
    {
        Symbol = symbol;
    }

    public IEnumerable<IInstanceMemberDeclarationNode> InstanceMembersNamed(StandardName named)
        => Members.OfType<IInstanceMemberDeclarationNode>().MembersNamed(ref instanceMembersByName, named);

    public IEnumerable<IAssociatedMemberDeclarationNode> AssociatedMembersNamed(StandardName named)
        => Members.OfType<IAssociatedMemberDeclarationNode>().MembersNamed(ref associatedMembersByName, named);
}
