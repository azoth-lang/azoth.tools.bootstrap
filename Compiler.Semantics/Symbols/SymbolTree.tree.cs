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
    typeof(IClassSymbolNode),
    typeof(IStructSymbolNode),
    typeof(ITraitSymbolNode),
    typeof(ITypeMemberSymbolNode))]
public partial interface ISymbolNode
{
    Symbol Symbol { get; }
}

[Closed(
    typeof(IDeclarationSymbolNode))]
public partial interface IChildSymbolNode : IChild<ISymbolNode>, ISymbolNode
{
    ISymbolNode Parent { get; }
    IPackageSymbolNode Package { get; }
}

public partial interface IPackageSymbolNode : ISymbolNode
{
    IdentifierName AliasOrName { get; }
    IdentifierName Name { get; }
    new PackageSymbol Symbol { get; }
    Symbol ISymbolNode.Symbol => Symbol;
    INamespaceSymbolNode GlobalNamespace { get; }
    INamespaceSymbolNode TestingGlobalNamespace { get; }
}

[Closed(
    typeof(INamespaceMemberSymbolNode),
    typeof(ITypeMemberSymbolNode))]
public partial interface IDeclarationSymbolNode : IChildSymbolNode
{
}

public partial interface INamespaceSymbolNode : INamespaceMemberSymbolNode
{
    new NamespaceSymbol Symbol { get; }
    Symbol ISymbolNode.Symbol => Symbol;
}

[Closed(
    typeof(INamespaceSymbolNode),
    typeof(ITypeSymbolNode),
    typeof(IFunctionSymbolNode))]
public partial interface INamespaceMemberSymbolNode : ISymbolNode, IDeclarationSymbolNode
{
}

[Closed(
    typeof(IClassSymbolNode),
    typeof(IStructSymbolNode),
    typeof(ITraitSymbolNode))]
public partial interface ITypeSymbolNode : INamespaceMemberSymbolNode, IClassMemberSymbolNode, ITraitMemberSymbolNode, IStructMemberSymbolNode
{
    IFixedList<ITypeMemberSymbolNode> Members { get; }
}

public partial interface IClassSymbolNode : ISymbolNode, ITypeSymbolNode
{
    new IFixedList<IClassMemberSymbolNode> Members { get; }
    IFixedList<ITypeMemberSymbolNode> ITypeSymbolNode.Members => Members;
}

public partial interface IStructSymbolNode : ISymbolNode, ITypeSymbolNode
{
    new IFixedList<IStructMemberSymbolNode> Members { get; }
    IFixedList<ITypeMemberSymbolNode> ITypeSymbolNode.Members => Members;
}

public partial interface ITraitSymbolNode : ISymbolNode, ITypeSymbolNode
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

public partial interface IFunctionSymbolNode : INamespaceMemberSymbolNode
{
}

