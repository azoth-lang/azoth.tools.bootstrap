using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;
using DotNet.Collections.Generic;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;

internal abstract class UserTypeSymbolNode : PackageFacetChildSymbolNode, IUserTypeDeclarationNode
{
    public override StandardName Name => base.Name!;
    private ValueAttribute<IFixedList<IGenericParameterDeclarationNode>> genericParameters;
    public IFixedList<IGenericParameterDeclarationNode> GenericParameters
        => genericParameters.TryGetValue(out var value) ? value
            : genericParameters.GetValue(GetGenericParameters);

    public override UserTypeSymbol Symbol { get; }
    public abstract IFixedSet<ITypeMemberDeclarationNode> Members { get; }
    private MultiMapHashSet<StandardName, IInstanceMemberDeclarationNode>? instanceMembersByName;
    private MultiMapHashSet<StandardName, IAssociatedMemberDeclarationNode>? associatedMembersByName;
    public abstract IFixedSet<ITypeMemberDeclarationNode> InclusiveMembers { get; }

    private protected UserTypeSymbolNode(UserTypeSymbol symbol)
    {
        Symbol = symbol;
    }

    private IFixedList<IGenericParameterDeclarationNode> GetGenericParameters()
    {
        var declarationNodes = InheritedSymbolTree().GetChildrenOf(Symbol)
            .OfType<GenericParameterTypeSymbol>().Select(SymbolNodeAspect.Symbol).WhereNotNull()
            .Cast<IGenericParameterDeclarationNode>();
        return ChildList.Attach(this, declarationNodes);
    }

    public IEnumerable<IInstanceMemberDeclarationNode> InclusiveInstanceMembersNamed(StandardName named)
        => Members.OfType<IInstanceMemberDeclarationNode>().MembersNamed(ref instanceMembersByName, named);

    public IEnumerable<IAssociatedMemberDeclarationNode> AssociatedMembersNamed(StandardName named)
        => Members.OfType<IAssociatedMemberDeclarationNode>().MembersNamed(ref associatedMembersByName, named);
}
