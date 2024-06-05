using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;

internal sealed class GenericParameterSymbolNode : PackageFacetChildSymbolNode, IGenericParameterSymbolNode
{
    public override GenericParameterTypeSymbol Symbol { get; }
    public override IdentifierName Name => Symbol.Name;
    StandardName INamedDeclarationNode.Name => Name;
    public IFixedSet<ITypeMemberDeclarationNode> Members
        => FixedSet.Empty<ITypeMemberDeclarationNode>();

    public GenericParameterSymbolNode(GenericParameterTypeSymbol symbol)
    {
        Symbol = symbol;
    }

    public IEnumerable<IInstanceMemberDeclarationNode> InstanceMembersNamed(StandardName named)
        => Enumerable.Empty<IInstanceMemberDeclarationNode>();

    public IEnumerable<IAssociatedMemberDeclarationNode> AssociatedMembersNamed(StandardName named)
        => Enumerable.Empty<IAssociatedMemberDeclarationNode>();
}
