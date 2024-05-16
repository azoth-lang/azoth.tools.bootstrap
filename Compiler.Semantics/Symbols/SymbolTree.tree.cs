using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

// ReSharper disable PartialTypeWithSinglePart

[Closed(
    typeof(IChildDeclarationNode),
    typeof(IPackageDeclarationNode),
    typeof(INamespaceMemberDeclarationNode),
    typeof(IFunctionDeclarationNode),
    typeof(IClassDeclarationNode),
    typeof(IStructDeclarationNode),
    typeof(ITraitDeclarationNode),
    typeof(ITypeMemberDeclarationNode),
    typeof(IMethodDeclarationNode),
    typeof(IConstructorDeclarationNode),
    typeof(IInitializerDeclarationNode),
    typeof(IFieldDeclarationNode),
    typeof(IAssociatedFunctionDeclarationNode),
    typeof(ITypeDeclarationNode))]
public partial interface IDeclarationNode
{
    Symbol Symbol { get; }
}

[Closed(
    typeof(INamedDeclarationNode),
    typeof(IPackageFacetDeclarationNode),
    typeof(IFacetChildDeclarationNode))]
public partial interface IChildDeclarationNode : IChild<IDeclarationNode>, IDeclarationNode
{
    IDeclarationNode Parent { get; }
    IPackageDeclarationNode Package { get; }
}

[Closed(
    typeof(IFunctionDeclarationNode),
    typeof(IMethodDeclarationNode),
    typeof(IFieldDeclarationNode),
    typeof(ITypeDeclarationNode))]
public partial interface INamedDeclarationNode : IChildDeclarationNode
{
    StandardName Name { get; }
}

public partial interface IPackageDeclarationNode : IDeclarationNode
{
    IdentifierName? AliasOrName { get; }
    IdentifierName Name { get; }
    new PackageSymbol Symbol { get; }
    Symbol IDeclarationNode.Symbol => Symbol;
    IPackageFacetDeclarationNode MainFacet { get; }
    IPackageFacetDeclarationNode TestingFacet { get; }
}

[Closed(
    typeof(IFunctionDeclarationNode),
    typeof(IUserTypeDeclarationNode))]
public partial interface IPackageMemberDeclarationNode : INamespaceMemberDeclarationNode
{
}

public partial interface IPackageFacetDeclarationNode : IChildDeclarationNode
{
    IdentifierName? PackageAliasOrName { get; }
    IdentifierName PackageName { get; }
    new PackageSymbol Symbol { get; }
    Symbol IDeclarationNode.Symbol => Symbol;
    INamespaceDeclarationNode GlobalNamespace { get; }
}

[Closed(
    typeof(INamespaceMemberDeclarationNode),
    typeof(ITypeMemberDeclarationNode))]
public partial interface IFacetChildDeclarationNode : IChildDeclarationNode
{
    StandardName? Name { get; }
    IPackageFacetDeclarationNode Facet { get; }
}

public partial interface INamespaceDeclarationNode : INamespaceMemberDeclarationNode
{
    new IdentifierName Name { get; }
    StandardName? IFacetChildDeclarationNode.Name => Name;
    new NamespaceSymbol Symbol { get; }
    Symbol IDeclarationNode.Symbol => Symbol;
    IFixedList<INamespaceMemberDeclarationNode> Members { get; }
    IFixedList<INamespaceMemberDeclarationNode> NestedMembers { get; }
}

[Closed(
    typeof(IPackageMemberDeclarationNode),
    typeof(INamespaceDeclarationNode))]
public partial interface INamespaceMemberDeclarationNode : IDeclarationNode, IFacetChildDeclarationNode
{
}

public partial interface IFunctionDeclarationNode : IDeclarationNode, IPackageMemberDeclarationNode, INamedDeclarationNode
{
    new FunctionSymbol Symbol { get; }
    Symbol IDeclarationNode.Symbol => Symbol;
}

[Closed(
    typeof(IClassDeclarationNode),
    typeof(IStructDeclarationNode),
    typeof(ITraitDeclarationNode))]
public partial interface IUserTypeDeclarationNode : IPackageMemberDeclarationNode, IClassMemberDeclarationNode, ITraitMemberDeclarationNode, IStructMemberDeclarationNode, ITypeDeclarationNode
{
    new UserTypeSymbol Symbol { get; }
    Symbol IDeclarationNode.Symbol => Symbol;
    TypeSymbol ITypeDeclarationNode.Symbol => Symbol;
    IFixedList<ITypeMemberDeclarationNode> Members { get; }
}

public partial interface IClassDeclarationNode : IDeclarationNode, IUserTypeDeclarationNode
{
    new IFixedList<IClassMemberDeclarationNode> Members { get; }
    IFixedList<ITypeMemberDeclarationNode> IUserTypeDeclarationNode.Members => Members;
}

public partial interface IStructDeclarationNode : IDeclarationNode, IUserTypeDeclarationNode
{
    new IFixedList<IStructMemberDeclarationNode> Members { get; }
    IFixedList<ITypeMemberDeclarationNode> IUserTypeDeclarationNode.Members => Members;
}

public partial interface ITraitDeclarationNode : IDeclarationNode, IUserTypeDeclarationNode
{
    new IFixedList<ITraitMemberDeclarationNode> Members { get; }
    IFixedList<ITypeMemberDeclarationNode> IUserTypeDeclarationNode.Members => Members;
}

public partial interface IGenericParameterDeclarationNode : ITypeDeclarationNode
{
    new IdentifierName Name { get; }
    StandardName INamedDeclarationNode.Name => Name;
}

[Closed(
    typeof(IClassMemberDeclarationNode),
    typeof(ITraitMemberDeclarationNode),
    typeof(IStructMemberDeclarationNode))]
public partial interface ITypeMemberDeclarationNode : IDeclarationNode, IFacetChildDeclarationNode
{
}

[Closed(
    typeof(IUserTypeDeclarationNode),
    typeof(IMethodDeclarationNode),
    typeof(IConstructorDeclarationNode),
    typeof(IFieldDeclarationNode),
    typeof(IAssociatedFunctionDeclarationNode))]
public partial interface IClassMemberDeclarationNode : ITypeMemberDeclarationNode
{
}

[Closed(
    typeof(IUserTypeDeclarationNode),
    typeof(IMethodDeclarationNode),
    typeof(IAssociatedFunctionDeclarationNode))]
public partial interface ITraitMemberDeclarationNode : ITypeMemberDeclarationNode
{
}

[Closed(
    typeof(IUserTypeDeclarationNode),
    typeof(IMethodDeclarationNode),
    typeof(IInitializerDeclarationNode),
    typeof(IFieldDeclarationNode),
    typeof(IAssociatedFunctionDeclarationNode))]
public partial interface IStructMemberDeclarationNode : ITypeMemberDeclarationNode
{
}

public partial interface IMethodDeclarationNode : IDeclarationNode, IClassMemberDeclarationNode, ITraitMemberDeclarationNode, IStructMemberDeclarationNode, INamedDeclarationNode
{
    new IdentifierName Name { get; }
    StandardName? IFacetChildDeclarationNode.Name => Name;
    StandardName INamedDeclarationNode.Name => Name;
    new MethodSymbol Symbol { get; }
    Symbol IDeclarationNode.Symbol => Symbol;
}

public partial interface IConstructorDeclarationNode : IDeclarationNode, IClassMemberDeclarationNode
{
    new IdentifierName? Name { get; }
    StandardName? IFacetChildDeclarationNode.Name => Name;
    new ConstructorSymbol Symbol { get; }
    Symbol IDeclarationNode.Symbol => Symbol;
}

public partial interface IInitializerDeclarationNode : IDeclarationNode, IStructMemberDeclarationNode
{
    new IdentifierName? Name { get; }
    StandardName? IFacetChildDeclarationNode.Name => Name;
    new InitializerSymbol Symbol { get; }
    Symbol IDeclarationNode.Symbol => Symbol;
}

public partial interface IFieldDeclarationNode : IDeclarationNode, INamedDeclarationNode, IClassMemberDeclarationNode, IStructMemberDeclarationNode
{
    new IdentifierName Name { get; }
    StandardName INamedDeclarationNode.Name => Name;
    StandardName? IFacetChildDeclarationNode.Name => Name;
    DataType Type { get; }
    new FieldSymbol Symbol { get; }
    Symbol IDeclarationNode.Symbol => Symbol;
}

public partial interface IAssociatedFunctionDeclarationNode : IDeclarationNode, IClassMemberDeclarationNode, ITraitMemberDeclarationNode, IStructMemberDeclarationNode
{
    new IdentifierName Name { get; }
    StandardName? IFacetChildDeclarationNode.Name => Name;
    new FunctionSymbol Symbol { get; }
    Symbol IDeclarationNode.Symbol => Symbol;
    FunctionType Type { get; }
}

[Closed(
    typeof(IUserTypeDeclarationNode),
    typeof(IGenericParameterDeclarationNode))]
public partial interface ITypeDeclarationNode : IDeclarationNode, INamedDeclarationNode
{
    new TypeSymbol Symbol { get; }
    Symbol IDeclarationNode.Symbol => Symbol;
}

