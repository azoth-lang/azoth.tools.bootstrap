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
    typeof(IClassDeclarationSymbolNode),
    typeof(IStructDeclarationSymbolNode),
    typeof(ITraitDeclarationSymbolNode),
    typeof(ITypeMemberSymbolNode),
    typeof(IFunctionSymbolNode),
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
    typeof(ITypeDeclarationSymbolNode),
    typeof(IFunctionSymbolNode))]
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

public partial interface IGenericParameterSymbolNode : ITypeSymbolNode
{
    new IdentifierName Name { get; }
    StandardName INamedSymbolNode.Name => Name;
}

[Closed(
    typeof(IClassDeclarationSymbolNode),
    typeof(IStructDeclarationSymbolNode),
    typeof(ITraitDeclarationSymbolNode))]
public partial interface ITypeDeclarationSymbolNode : IPackageMemberSymbolNode, IClassMemberSymbolNode, ITraitMemberSymbolNode, IStructMemberSymbolNode, ITypeSymbolNode
{
    new UserTypeSymbol Symbol { get; }
    Symbol ISymbolNode.Symbol => Symbol;
    TypeSymbol ITypeSymbolNode.Symbol => Symbol;
    IFixedList<ITypeMemberSymbolNode> Members { get; }
}

public partial interface IClassDeclarationSymbolNode : ISymbolNode, ITypeDeclarationSymbolNode
{
    new IFixedList<IClassMemberSymbolNode> Members { get; }
    IFixedList<ITypeMemberSymbolNode> ITypeDeclarationSymbolNode.Members => Members;
}

public partial interface IStructDeclarationSymbolNode : ISymbolNode, ITypeDeclarationSymbolNode
{
    new IFixedList<IStructMemberSymbolNode> Members { get; }
    IFixedList<ITypeMemberSymbolNode> ITypeDeclarationSymbolNode.Members => Members;
}

public partial interface ITraitDeclarationSymbolNode : ISymbolNode, ITypeDeclarationSymbolNode
{
    new IFixedList<ITraitMemberSymbolNode> Members { get; }
    IFixedList<ITypeMemberSymbolNode> ITypeDeclarationSymbolNode.Members => Members;
}

[Closed(
    typeof(IClassMemberSymbolNode),
    typeof(ITraitMemberSymbolNode),
    typeof(IStructMemberSymbolNode))]
public partial interface ITypeMemberSymbolNode : ISymbolNode, IDeclarationSymbolNode
{
}

[Closed(
    typeof(ITypeDeclarationSymbolNode))]
public partial interface IClassMemberSymbolNode : ITypeMemberSymbolNode
{
}

[Closed(
    typeof(ITypeDeclarationSymbolNode))]
public partial interface ITraitMemberSymbolNode : ITypeMemberSymbolNode
{
}

[Closed(
    typeof(ITypeDeclarationSymbolNode))]
public partial interface IStructMemberSymbolNode : ITypeMemberSymbolNode
{
}

public partial interface IFunctionSymbolNode : ISymbolNode, IPackageMemberSymbolNode
{
    new FunctionSymbol Symbol { get; }
    Symbol ISymbolNode.Symbol => Symbol;
}

[Closed(
    typeof(IGenericParameterSymbolNode),
    typeof(ITypeDeclarationSymbolNode))]
public partial interface ITypeSymbolNode : ISymbolNode, INamedSymbolNode
{
    new TypeSymbol Symbol { get; }
    Symbol ISymbolNode.Symbol => Symbol;
}

