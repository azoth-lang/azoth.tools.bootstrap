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
    typeof(IDeclarationWithMembersSymbolNode),
    typeof(INamespaceMemberSymbolNode),
    typeof(ITypeSymbolNode),
    typeof(ITypeMemberSymbolNode),
    typeof(IFunctionSymbolNode))]
public partial interface ISymbolNode
{
    Symbol Symbol { get; }
}

[Closed(
    typeof(IPackageFacetSymbolNode),
    typeof(IDeclarationSymbolNode))]
public partial interface IChildSymbolNode : IChild<ISymbolNode>, ISymbolNode
{
    ISymbolNode Parent { get; }
    IPackageSymbolNode Package { get; }
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
    typeof(ITypeSymbolNode),
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
    typeof(IDeclarationWithMembersSymbolNode),
    typeof(INamespaceMemberSymbolNode),
    typeof(ITypeMemberSymbolNode))]
public partial interface IDeclarationSymbolNode : IChildSymbolNode
{
    StandardName Name { get; }
    IPackageFacetSymbolNode Facet { get; }
}

public partial interface IDeclarationWithMembersSymbolNode : ISymbolNode, IDeclarationSymbolNode
{
    IFixedList<IDeclarationSymbolNode> Members { get; }
}

public partial interface INamespaceSymbolNode : INamespaceMemberSymbolNode
{
    new IdentifierName Name { get; }
    StandardName IDeclarationSymbolNode.Name => Name;
    new NamespaceSymbol Symbol { get; }
    Symbol ISymbolNode.Symbol => Symbol;
    IFixedList<INamespaceMemberSymbolNode> Members { get; }
}

[Closed(
    typeof(IPackageMemberSymbolNode),
    typeof(INamespaceSymbolNode))]
public partial interface INamespaceMemberSymbolNode : ISymbolNode, IDeclarationSymbolNode
{
}

[Closed(
    typeof(IClassSymbolNode),
    typeof(IStructSymbolNode),
    typeof(ITraitSymbolNode))]
public partial interface ITypeSymbolNode : ISymbolNode, IPackageMemberSymbolNode, IClassMemberSymbolNode, ITraitMemberSymbolNode, IStructMemberSymbolNode
{
    new UserTypeSymbol Symbol { get; }
    Symbol ISymbolNode.Symbol => Symbol;
    IFixedList<ITypeMemberSymbolNode> Members { get; }
}

public partial interface IClassSymbolNode : ITypeSymbolNode
{
    new IFixedList<IClassMemberSymbolNode> Members { get; }
    IFixedList<ITypeMemberSymbolNode> ITypeSymbolNode.Members => Members;
}

public partial interface IStructSymbolNode : ITypeSymbolNode
{
    new IFixedList<IStructMemberSymbolNode> Members { get; }
    IFixedList<ITypeMemberSymbolNode> ITypeSymbolNode.Members => Members;
}

public partial interface ITraitSymbolNode : ITypeSymbolNode
{
    new IFixedList<ITraitMemberSymbolNode> Members { get; }
    IFixedList<ITypeMemberSymbolNode> ITypeSymbolNode.Members => Members;
}

[Closed(
    typeof(IClassMemberSymbolNode),
    typeof(ITraitMemberSymbolNode),
    typeof(IStructMemberSymbolNode))]
public partial interface ITypeMemberSymbolNode : ISymbolNode, IDeclarationSymbolNode
{
}

[Closed(
    typeof(ITypeSymbolNode))]
public partial interface IClassMemberSymbolNode : ITypeMemberSymbolNode
{
}

[Closed(
    typeof(ITypeSymbolNode))]
public partial interface ITraitMemberSymbolNode : ITypeMemberSymbolNode
{
}

[Closed(
    typeof(ITypeSymbolNode))]
public partial interface IStructMemberSymbolNode : ITypeMemberSymbolNode
{
}

public partial interface IFunctionSymbolNode : ISymbolNode, IPackageMemberSymbolNode
{
    new FunctionSymbol Symbol { get; }
    Symbol ISymbolNode.Symbol => Symbol;
}

