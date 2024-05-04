using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics;

// ReSharper disable PartialTypeWithSinglePart

[Closed(
    typeof(IChildNode),
    typeof(IPackageNode),
    typeof(IPackageMemberDeclarationNode),
    typeof(ICompilationUnitNode),
    typeof(IUsingDirectiveNode),
    typeof(IDeclarationNode),
    typeof(INamespaceDeclarationNode),
    typeof(IClassDeclarationNode),
    typeof(IStructDeclarationNode),
    typeof(ITraitDeclarationNode),
    typeof(IGenericParameterNode),
    typeof(ISupertypeNameNode),
    typeof(IClassMemberDeclarationNode),
    typeof(ITraitMemberDeclarationNode),
    typeof(IStructMemberDeclarationNode),
    typeof(ICapabilityConstraintNode))]
public partial interface ISemanticNode
{
    ISyntax Syntax { get; }
}

[Closed(
    typeof(IPackageReferenceNode),
    typeof(IPackageFacetNode),
    typeof(ICodeNode))]
public partial interface IChildNode : IChild<ISemanticNode>, ISemanticNode
{
    ISemanticNode Parent { get; }
    IPackageNode Package { get; }
}

public partial interface IPackageNode : ISemanticNode
{
    new IPackageSyntax Syntax { get; }
    ISyntax ISemanticNode.Syntax => Syntax;
    IdentifierName Name { get; }
    PackageSymbol Symbol { get; }
    IPackageSymbolNode SymbolNode { get; }
    IFixedSet<IPackageReferenceNode> References { get; }
    FixedDictionary<IdentifierName,IPackageSymbolNode> SymbolNodes { get; }
    IPackageFacetNode MainFacet { get; }
    IPackageFacetNode TestingFacet { get; }
}

public partial interface IPackageReferenceNode : IChildNode
{
    new IPackageReferenceSyntax Syntax { get; }
    ISyntax ISemanticNode.Syntax => Syntax;
    IPackageSymbolNode SymbolNode { get; }
    IdentifierName AliasOrName { get; }
    IPackageSymbols PackageSymbols { get; }
    bool IsTrusted { get; }
}

public partial interface IPackageFacetNode : IChildNode
{
    new IPackageSyntax Syntax { get; }
    ISyntax ISemanticNode.Syntax => Syntax;
    IdentifierName PackageName { get; }
    PackageSymbol PackageSymbol { get; }
    IPackageFacetSymbolNode SymbolNode { get; }
    PackageNameScope PackageNameScope { get; }
    IFixedSet<ICompilationUnitNode> CompilationUnits { get; }
    IFixedSet<IPackageMemberDeclarationNode> Declarations { get; }
}

[Closed(
    typeof(ITypeDeclarationNode),
    typeof(IFunctionDeclarationNode))]
public partial interface IPackageMemberDeclarationNode : ISemanticNode, INamespaceMemberDeclarationNode
{
    CodeFile File { get; }
}

[Closed(
    typeof(ICompilationUnitNode),
    typeof(IUsingDirectiveNode),
    typeof(IDeclarationNode),
    typeof(IGenericParameterNode),
    typeof(ISupertypeNameNode),
    typeof(ICapabilityConstraintNode))]
public partial interface ICodeNode : IChildNode
{
    new IConcreteSyntax Syntax { get; }
    ISyntax ISemanticNode.Syntax => Syntax;
}

public partial interface ICompilationUnitNode : ISemanticNode, ICodeNode
{
    new ICompilationUnitSyntax Syntax { get; }
    ISyntax ISemanticNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    CodeFile File { get; }
    IPackageFacetSymbolNode ContainingSymbolNode { get; }
    NamespaceSymbol ContainingSymbol { get; }
    NamespaceName ImplicitNamespaceName { get; }
    INamespaceSymbolNode ImplicitNamespaceSymbolNode { get; }
    NamespaceSymbol ImplicitNamespaceSymbol { get; }
    IFixedList<IUsingDirectiveNode> UsingDirectives { get; }
    IFixedList<INamespaceMemberDeclarationNode> Declarations { get; }
    NamespaceScope ContainingLexicalScope { get; }
    NamespaceScope LexicalScope { get; }
}

public partial interface IUsingDirectiveNode : ISemanticNode, ICodeNode
{
    new IUsingDirectiveSyntax Syntax { get; }
    ISyntax ISemanticNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    NamespaceName Name { get; }
}

[Closed(
    typeof(INamespaceMemberDeclarationNode),
    typeof(ITypeMemberDeclarationNode))]
public partial interface IDeclarationNode : ISemanticNode, ICodeNode
{
    new IDeclarationSyntax Syntax { get; }
    ISyntax ISemanticNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    ISymbolNode ContainingSymbolNode { get; }
    Symbol ContainingSymbol { get; }
    LexicalScope ContainingLexicalScope { get; }
}

public partial interface INamespaceDeclarationNode : ISemanticNode, INamespaceMemberDeclarationNode
{
    new INamespaceDeclarationSyntax Syntax { get; }
    ISyntax ISemanticNode.Syntax => Syntax;
    IDeclarationSyntax IDeclarationNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    bool IsGlobalQualified { get; }
    NamespaceName DeclaredNames { get; }
    IFixedList<IUsingDirectiveNode> UsingDirectives { get; }
    IFixedList<INamespaceMemberDeclarationNode> Declarations { get; }
    new INamespaceSymbolNode ContainingSymbolNode { get; }
    ISymbolNode IDeclarationNode.ContainingSymbolNode => ContainingSymbolNode;
    new NamespaceSymbol ContainingSymbol { get; }
    Symbol IDeclarationNode.ContainingSymbol => ContainingSymbol;
    INamespaceSymbolNode SymbolNode { get; }
    NamespaceSymbol Symbol { get; }
    new NamespaceScope ContainingLexicalScope { get; }
    LexicalScope IDeclarationNode.ContainingLexicalScope => ContainingLexicalScope;
}

[Closed(
    typeof(IPackageMemberDeclarationNode),
    typeof(INamespaceDeclarationNode))]
public partial interface INamespaceMemberDeclarationNode : IDeclarationNode
{
}

[Closed(
    typeof(IClassDeclarationNode),
    typeof(IStructDeclarationNode),
    typeof(ITraitDeclarationNode))]
public partial interface ITypeDeclarationNode : IPackageMemberDeclarationNode, IClassMemberDeclarationNode, ITraitMemberDeclarationNode, IStructMemberDeclarationNode
{
    new ITypeDeclarationSyntax Syntax { get; }
    ISyntax ISemanticNode.Syntax => Syntax;
    IDeclarationSyntax IDeclarationNode.Syntax => Syntax;
    IClassMemberDeclarationSyntax IClassMemberDeclarationNode.Syntax => Syntax;
    ITraitMemberDeclarationSyntax ITraitMemberDeclarationNode.Syntax => Syntax;
    IStructMemberDeclarationSyntax IStructMemberDeclarationNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    ITypeMemberDeclarationSyntax ITypeMemberDeclarationNode.Syntax => Syntax;
    StandardName Name { get; }
    ITypeSymbolNode SymbolNode { get; }
    IFixedList<IGenericParameterNode> GenericParameters { get; }
    IFixedList<ISupertypeNameNode> SupertypeNames { get; }
    IFixedList<ITypeMemberDeclarationNode> Members { get; }
}

public partial interface IClassDeclarationNode : ISemanticNode, ITypeDeclarationNode
{
    new IClassDeclarationSyntax Syntax { get; }
    ISyntax ISemanticNode.Syntax => Syntax;
    ITypeDeclarationSyntax ITypeDeclarationNode.Syntax => Syntax;
    IDeclarationSyntax IDeclarationNode.Syntax => Syntax;
    IClassMemberDeclarationSyntax IClassMemberDeclarationNode.Syntax => Syntax;
    ITraitMemberDeclarationSyntax ITraitMemberDeclarationNode.Syntax => Syntax;
    IStructMemberDeclarationSyntax IStructMemberDeclarationNode.Syntax => Syntax;
    bool IsAbstract { get; }
    ISupertypeNameNode? BaseTypeName { get; }
    new IFixedList<IClassMemberDeclarationNode> Members { get; }
    IFixedList<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
}

public partial interface IStructDeclarationNode : ISemanticNode, ITypeDeclarationNode
{
    new IStructDeclarationSyntax Syntax { get; }
    ISyntax ISemanticNode.Syntax => Syntax;
    ITypeDeclarationSyntax ITypeDeclarationNode.Syntax => Syntax;
    IDeclarationSyntax IDeclarationNode.Syntax => Syntax;
    IClassMemberDeclarationSyntax IClassMemberDeclarationNode.Syntax => Syntax;
    ITraitMemberDeclarationSyntax ITraitMemberDeclarationNode.Syntax => Syntax;
    IStructMemberDeclarationSyntax IStructMemberDeclarationNode.Syntax => Syntax;
    new IFixedList<IStructMemberDeclarationNode> Members { get; }
    IFixedList<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
}

public partial interface ITraitDeclarationNode : ISemanticNode, ITypeDeclarationNode
{
    new ITraitDeclarationSyntax Syntax { get; }
    ISyntax ISemanticNode.Syntax => Syntax;
    ITypeDeclarationSyntax ITypeDeclarationNode.Syntax => Syntax;
    IDeclarationSyntax IDeclarationNode.Syntax => Syntax;
    IClassMemberDeclarationSyntax IClassMemberDeclarationNode.Syntax => Syntax;
    ITraitMemberDeclarationSyntax ITraitMemberDeclarationNode.Syntax => Syntax;
    IStructMemberDeclarationSyntax IStructMemberDeclarationNode.Syntax => Syntax;
    new IFixedList<ITraitMemberDeclarationNode> Members { get; }
    IFixedList<ITypeMemberDeclarationNode> ITypeDeclarationNode.Members => Members;
}

public partial interface IGenericParameterNode : ISemanticNode, ICodeNode
{
    new IGenericParameterSyntax Syntax { get; }
    ISyntax ISemanticNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    ICapabilityConstraintNode Constraint { get; }
    IdentifierName Name { get; }
    ParameterIndependence Independence { get; }
    ParameterVariance Variance { get; }
}

public partial interface ISupertypeNameNode : ISemanticNode, ICodeNode
{
    new ISupertypeNameSyntax Syntax { get; }
    ISyntax ISemanticNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    TypeName Name { get; }
}

[Closed(
    typeof(IClassMemberDeclarationNode),
    typeof(ITraitMemberDeclarationNode),
    typeof(IStructMemberDeclarationNode))]
public partial interface ITypeMemberDeclarationNode : IDeclarationNode
{
    new ITypeMemberDeclarationSyntax Syntax { get; }
    IDeclarationSyntax IDeclarationNode.Syntax => Syntax;
}

[Closed(
    typeof(ITypeDeclarationNode))]
public partial interface IClassMemberDeclarationNode : ISemanticNode, ITypeMemberDeclarationNode
{
    new IClassMemberDeclarationSyntax Syntax { get; }
    ISyntax ISemanticNode.Syntax => Syntax;
    ITypeMemberDeclarationSyntax ITypeMemberDeclarationNode.Syntax => Syntax;
    IDeclarationSyntax IDeclarationNode.Syntax => Syntax;
}

[Closed(
    typeof(ITypeDeclarationNode))]
public partial interface ITraitMemberDeclarationNode : ISemanticNode, ITypeMemberDeclarationNode
{
    new ITraitMemberDeclarationSyntax Syntax { get; }
    ISyntax ISemanticNode.Syntax => Syntax;
    ITypeMemberDeclarationSyntax ITypeMemberDeclarationNode.Syntax => Syntax;
    IDeclarationSyntax IDeclarationNode.Syntax => Syntax;
}

[Closed(
    typeof(ITypeDeclarationNode))]
public partial interface IStructMemberDeclarationNode : ISemanticNode, ITypeMemberDeclarationNode
{
    new IStructMemberDeclarationSyntax Syntax { get; }
    ISyntax ISemanticNode.Syntax => Syntax;
    ITypeMemberDeclarationSyntax ITypeMemberDeclarationNode.Syntax => Syntax;
    IDeclarationSyntax IDeclarationNode.Syntax => Syntax;
}

public partial interface IFunctionDeclarationNode : IPackageMemberDeclarationNode
{
    new IFunctionDeclarationSyntax Syntax { get; }
    ISyntax ISemanticNode.Syntax => Syntax;
    IDeclarationSyntax IDeclarationNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    new INamespaceSymbolNode ContainingSymbolNode { get; }
    ISymbolNode IDeclarationNode.ContainingSymbolNode => ContainingSymbolNode;
    new NamespaceSymbol ContainingSymbol { get; }
    Symbol IDeclarationNode.ContainingSymbol => ContainingSymbol;
    StandardName Name { get; }
    new NamespaceScope ContainingLexicalScope { get; }
    LexicalScope IDeclarationNode.ContainingLexicalScope => ContainingLexicalScope;
}

[Closed(
    typeof(ICapabilitySetNode),
    typeof(ICapabilityNode))]
public partial interface ICapabilityConstraintNode : ISemanticNode, ICodeNode
{
    new ICapabilityConstraintSyntax Syntax { get; }
    ISyntax ISemanticNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    Compiler.Types.Capabilities.ICapabilityConstraint Constraint { get; }
}

public partial interface ICapabilitySetNode : ICapabilityConstraintNode
{
    new ICapabilitySetSyntax Syntax { get; }
    ICapabilityConstraintSyntax ICapabilityConstraintNode.Syntax => Syntax;
    new CapabilitySet Constraint { get; }
    Compiler.Types.Capabilities.ICapabilityConstraint ICapabilityConstraintNode.Constraint => Constraint;
}

public partial interface ICapabilityNode : ICapabilityConstraintNode
{
    new ICapabilitySyntax Syntax { get; }
    ICapabilityConstraintSyntax ICapabilityConstraintNode.Syntax => Syntax;
    Capability Capability { get; }
    new Capability Constraint { get; }
    Compiler.Types.Capabilities.ICapabilityConstraint ICapabilityConstraintNode.Constraint => Constraint;
}

