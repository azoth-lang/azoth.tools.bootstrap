using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;

internal sealed class GenericParameterSymbolNode : PackageFacetChildSymbolNode, IGenericParameterSymbolNode
{
    public override GenericParameterTypeSymbol Symbol { get; }
    public override IdentifierName Name => Symbol.Name;
    TypeName INamedDeclarationNode.Name => Name;
    public IFixedSet<BareReferenceType> Supertypes
        => Symbol.GetDeclaredType()?.Supertypes ?? FixedSet.Empty<BareReferenceType>();
    public bool SupertypesFormCycle => false;
    public IFixedSet<ITypeMemberDeclarationNode> Members
        => FixedSet.Empty<ITypeMemberDeclarationNode>();
    public IFixedSet<ITypeMemberDeclarationNode> InclusiveMembers
        => FixedSet.Empty<ITypeMemberDefinitionNode>();

    public GenericParameterSymbolNode(GenericParameterTypeSymbol symbol)
    {
        Symbol = symbol;
    }

    public IEnumerable<IInstanceMemberDeclarationNode> InclusiveInstanceMembersNamed(StandardName named)
        => Enumerable.Empty<IInstanceMemberDeclarationNode>();

    public IEnumerable<IAssociatedMemberDeclarationNode> AssociatedMembersNamed(StandardName named)
        => Enumerable.Empty<IAssociatedMemberDeclarationNode>();
}
