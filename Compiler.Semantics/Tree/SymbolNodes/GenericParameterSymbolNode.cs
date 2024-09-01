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
        => Symbol.GetDeclaredType()?.Supertypes ?? [];
    public IFixedSet<ITypeMemberSymbolNode> Members => [];
    public IFixedSet<ITypeMemberDeclarationNode> InclusiveMembers => [];

    public GenericParameterSymbolNode(GenericParameterTypeSymbol symbol)
    {
        Symbol = symbol;
    }
}
