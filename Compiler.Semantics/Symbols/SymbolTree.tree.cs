using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

// ReSharper disable PartialTypeWithSinglePart

[Closed(
    typeof(IChildSymbolNode),
    typeof(IPackageSymbolNode),
    typeof(INamespaceMemberSymbolNode),
    typeof(IFunctionSymbolNode),
    typeof(IClassSymbolNode),
    typeof(IStructSymbolNode),
    typeof(ITraitSymbolNode),
    typeof(ITypeMemberSymbolNode),
    typeof(ITypeSymbolNode))]
public partial interface ISymbolNode
{
    Symbol Symbol { get; }
}

[Closed(
    typeof(INamedSymbolNode),
    typeof(IPackageFacetSymbolNode),
    typeof(IDeclarationSymbolNode))]
public partial interface IChildSymbolNode : IChild<ISymbolNode>, ISymbolNode
{
    ISymbolNode Parent { get; }
    IPackageSymbolNode Package { get; }
}

[Closed(
    typeof(ITypeSymbolNode))]
public partial interface INamedSymbolNode : IChildSymbolNode
{
    StandardName Name { get; }
}

public partial interface IPackageSymbolNode : ISymbolNode
{
    IdentifierName? AliasOrName { get; }
    IdentifierName Name { get; }
    new PackageSymbol Symbol { get; }
    Symbol ISymbolNode.Symbol => Symbol;
    IPackageFacetSymbolNode MainFacet { get; }
    IPackageFacetSymbolNode TestingFacet { get; }
}

[Closed(
    typeof(IFunctionSymbolNode),
    typeof(IUserTypeSymbolNode))]
public partial interface IPackageMemberSymbolNode : INamespaceMemberSymbolNode
{
}

public partial interface IPackageFacetSymbolNode : IChildSymbolNode
{
    IdentifierName? PackageAliasOrName { get; }
    IdentifierName PackageName { get; }
    new PackageSymbol Symbol { get; }
    Symbol ISymbolNode.Symbol => Symbol;
    INamespaceSymbolNode GlobalNamespace { get; }
}

[Closed(
    typeof(INamespaceMemberSymbolNode),
    typeof(ITypeMemberSymbolNode))]
public partial interface IDeclarationSymbolNode : IChildSymbolNode
{
    StandardName Name { get; }
    IPackageFacetSymbolNode Facet { get; }
}

public partial interface INamespaceSymbolNode : INamespaceMemberSymbolNode
{
    new IdentifierName Name { get; }
    StandardName IDeclarationSymbolNode.Name => Name;
    new NamespaceSymbol Symbol { get; }
    Symbol ISymbolNode.Symbol => Symbol;
    IFixedList<INamespaceMemberSymbolNode> Members { get; }
    IFixedList<INamespaceMemberSymbolNode> NestedMembers { get; }
}

[Closed(
    typeof(IPackageMemberSymbolNode),
    typeof(INamespaceSymbolNode))]
public partial interface INamespaceMemberSymbolNode : ISymbolNode, IDeclarationSymbolNode
{
}

public partial interface IFunctionSymbolNode : ISymbolNode, IPackageMemberSymbolNode
{
    new FunctionSymbol Symbol { get; }
    Symbol ISymbolNode.Symbol => Symbol;
}

[Closed(
    typeof(IClassSymbolNode),
    typeof(IStructSymbolNode),
    typeof(ITraitSymbolNode))]
public partial interface IUserTypeSymbolNode : IPackageMemberSymbolNode, IClassMemberSymbolNode, ITraitMemberSymbolNode, IStructMemberSymbolNode, ITypeSymbolNode
{
    new UserTypeSymbol Symbol { get; }
    Symbol ISymbolNode.Symbol => Symbol;
    TypeSymbol ITypeSymbolNode.Symbol => Symbol;
    IFixedList<ITypeMemberSymbolNode> Members { get; }
}

public partial interface IClassSymbolNode : ISymbolNode, IUserTypeSymbolNode
{
    new IFixedList<IClassMemberSymbolNode> Members { get; }
    IFixedList<ITypeMemberSymbolNode> IUserTypeSymbolNode.Members => Members;
}

public partial interface IStructSymbolNode : ISymbolNode, IUserTypeSymbolNode
{
    new IFixedList<IStructMemberSymbolNode> Members { get; }
    IFixedList<ITypeMemberSymbolNode> IUserTypeSymbolNode.Members => Members;
}

public partial interface ITraitSymbolNode : ISymbolNode, IUserTypeSymbolNode
{
    new IFixedList<ITraitMemberSymbolNode> Members { get; }
    IFixedList<ITypeMemberSymbolNode> IUserTypeSymbolNode.Members => Members;
}

public partial interface IGenericParameterSymbolNode : ITypeSymbolNode
{
    new IdentifierName Name { get; }
    StandardName INamedSymbolNode.Name => Name;
}

[Closed(
    typeof(IClassMemberSymbolNode),
    typeof(ITraitMemberSymbolNode),
    typeof(IStructMemberSymbolNode))]
public partial interface ITypeMemberSymbolNode : ISymbolNode, IDeclarationSymbolNode
{
}

[Closed(
    typeof(IUserTypeSymbolNode))]
public partial interface IClassMemberSymbolNode : ITypeMemberSymbolNode
{
}

[Closed(
    typeof(IUserTypeSymbolNode))]
public partial interface ITraitMemberSymbolNode : ITypeMemberSymbolNode
{
}

[Closed(
    typeof(IUserTypeSymbolNode))]
public partial interface IStructMemberSymbolNode : ITypeMemberSymbolNode
{
}

[Closed(
    typeof(IUserTypeSymbolNode),
    typeof(IGenericParameterSymbolNode))]
public partial interface ITypeSymbolNode : ISymbolNode, INamedSymbolNode
{
    new TypeSymbol Symbol { get; }
    Symbol ISymbolNode.Symbol => Symbol;
}

