using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
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
    typeof(IFunctionDeclarationNode),
    typeof(ICapabilityConstraintNode))]
public partial interface ISemanticNode
{
    ISyntax Syntax { get; }
}

[Closed(
    typeof(IPackageReferenceNode),
    typeof(ICodeNode))]
public partial interface IChildNode : IChild<ISemanticNode>, ISemanticNode
{
    ISemanticNode Parent { get; }
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
    IFixedSet<ICompilationUnitNode> CompilationUnits { get; }
    IFixedSet<ICompilationUnitNode> TestingCompilationUnits { get; }
}

public partial interface IPackageReferenceNode : IChildNode
{
    new IPackageReferenceSyntax Syntax { get; }
    ISyntax ISemanticNode.Syntax => Syntax;
    IPackageSymbolNode SymbolNode { get; }
    IdentifierName AliasOrName { get; }
    IPackageSymbols Package { get; }
    bool IsTrusted { get; }
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
    NamespaceName ImplicitNamespaceName { get; }
    IFixedList<IUsingDirectiveNode> UsingDirectives { get; }
    IFixedList<INamespaceMemberDeclarationNode> Declarations { get; }
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
    NamespaceSymbol? ContainingNamespace { get; }
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
    new NamespaceSymbol ContainingNamespace { get; }
    NamespaceSymbol? IDeclarationNode.ContainingNamespace => ContainingNamespace;
    NamespaceSymbol Symbol { get; }
}

[Closed(
    typeof(INamespaceDeclarationNode),
    typeof(ITypeDeclarationNode),
    typeof(IFunctionDeclarationNode))]
public partial interface INamespaceMemberDeclarationNode : IDeclarationNode
{
}

[Closed(
    typeof(IClassDeclarationNode),
    typeof(IStructDeclarationNode),
    typeof(ITraitDeclarationNode))]
public partial interface ITypeDeclarationNode : INamespaceMemberDeclarationNode, IClassMemberDeclarationNode, ITraitMemberDeclarationNode, IStructMemberDeclarationNode
{
    new ITypeDeclarationSyntax Syntax { get; }
    IDeclarationSyntax IDeclarationNode.Syntax => Syntax;
    IClassMemberDeclarationSyntax IClassMemberDeclarationNode.Syntax => Syntax;
    ITraitMemberDeclarationSyntax ITraitMemberDeclarationNode.Syntax => Syntax;
    IStructMemberDeclarationSyntax IStructMemberDeclarationNode.Syntax => Syntax;
    ISyntax ISemanticNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    ITypeMemberDeclarationSyntax ITypeMemberDeclarationNode.Syntax => Syntax;
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

public partial interface IFunctionDeclarationNode : ISemanticNode, INamespaceMemberDeclarationNode
{
    new IFunctionDeclarationSyntax Syntax { get; }
    ISyntax ISemanticNode.Syntax => Syntax;
    IDeclarationSyntax IDeclarationNode.Syntax => Syntax;
    IConcreteSyntax ICodeNode.Syntax => Syntax;
    new NamespaceSymbol ContainingNamespace { get; }
    NamespaceSymbol? IDeclarationNode.ContainingNamespace => ContainingNamespace;
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

