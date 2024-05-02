using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics;

// ReSharper disable PartialTypeWithSinglePart

[Closed(
    typeof(IPackage),
    typeof(IPackageReference),
    typeof(ICode),
    typeof(INamespaceMemberDeclaration),
    typeof(IClassDeclaration),
    typeof(IStructDeclaration),
    typeof(ITraitDeclaration),
    typeof(ITypeMemberDeclaration),
    typeof(ICapabilitySet),
    typeof(ICapability))]
public partial interface ISemanticNode
{
    ISyntax Syntax { get; }
}

public partial interface IPackage : ISemanticNode
{
    new IPackageSyntax Syntax { get; }
    IdentifierName Name { get; }
    PackageSymbol Symbol { get; }
    IFixedSet<IPackageReference> References { get; }
    IFixedSet<ICompilationUnit> CompilationUnits { get; }
    IFixedSet<ICompilationUnit> TestingCompilationUnits { get; }
}

public partial interface IPackageReference : ISemanticNode
{
    new IPackageReferenceSyntax Syntax { get; }
    IdentifierName AliasOrName { get; }
    IPackageSymbols Package { get; }
    bool IsTrusted { get; }
}

[Closed(
    typeof(ICompilationUnit),
    typeof(IUsingDirective),
    typeof(IDeclaration),
    typeof(IGenericParameter),
    typeof(IUnresolvedSupertypeName),
    typeof(ICapabilityConstraint))]
public partial interface ICode : ISemanticNode
{
    new IConcreteSyntax Syntax { get; }
}

public partial interface ICompilationUnit : ICode
{
    new ICompilationUnitSyntax Syntax { get; }
    CodeFile File { get; }
    NamespaceName ImplicitNamespaceName { get; }
    IFixedList<IUsingDirective> UsingDirectives { get; }
    IFixedList<INamespaceMemberDeclaration> Declarations { get; }
}

public partial interface IUsingDirective : ICode
{
    new IUsingDirectiveSyntax Syntax { get; }
    NamespaceName Name { get; }
}

[Closed(
    typeof(INamespaceMemberDeclaration),
    typeof(ITypeMemberDeclaration))]
public partial interface IDeclaration : ICode
{
    new IDeclarationSyntax Syntax { get; }
}

public partial interface INamespaceDeclaration : INamespaceMemberDeclaration
{
    new INamespaceDeclarationSyntax Syntax { get; }
    bool IsGlobalQualified { get; }
    NamespaceName DeclaredNames { get; }
    IFixedList<IUsingDirective> UsingDirectives { get; }
    IFixedList<INamespaceMemberDeclaration> Declarations { get; }
}

[Closed(
    typeof(INamespaceDeclaration),
    typeof(ITypeDeclaration))]
public partial interface INamespaceMemberDeclaration : ISemanticNode, IDeclaration
{
}

[Closed(
    typeof(IClassDeclaration),
    typeof(IStructDeclaration),
    typeof(ITraitDeclaration))]
public partial interface ITypeDeclaration : INamespaceMemberDeclaration, IClassMemberDeclaration, ITraitMemberDeclaration, IStructMemberDeclaration
{
    new ITypeDeclarationSyntax Syntax { get; }
    IFixedList<IGenericParameter> GenericParameters { get; }
    IFixedList<IUnresolvedSupertypeName> SupertypeNames { get; }
    IFixedList<ITypeMemberDeclaration> Members { get; }
}

public partial interface IClassDeclaration : ISemanticNode, ITypeDeclaration
{
    new IClassDeclarationSyntax Syntax { get; }
    bool IsAbstract { get; }
    IUnresolvedSupertypeName? BaseTypeName { get; }
    new IFixedList<IClassMemberDeclaration> Members { get; }
}

public partial interface IStructDeclaration : ISemanticNode, ITypeDeclaration
{
    new IStructDeclarationSyntax Syntax { get; }
    new IFixedList<IStructMemberDeclaration> Members { get; }
}

public partial interface ITraitDeclaration : ISemanticNode, ITypeDeclaration
{
    new ITraitDeclarationSyntax Syntax { get; }
    new IFixedList<ITraitMemberDeclaration> Members { get; }
}

public partial interface IGenericParameter : ICode
{
    new IGenericParameterSyntax Syntax { get; }
    ICapabilityConstraint Constraint { get; }
    IdentifierName Name { get; }
    ParameterIndependence Independence { get; }
    ParameterVariance Variance { get; }
}

public partial interface IUnresolvedSupertypeName : ICode
{
    new ISupertypeNameSyntax Syntax { get; }
    TypeName Name { get; }
}

[Closed(
    typeof(IClassMemberDeclaration),
    typeof(ITraitMemberDeclaration),
    typeof(IStructMemberDeclaration))]
public partial interface ITypeMemberDeclaration : ISemanticNode, IDeclaration
{
    new ITypeMemberDeclarationSyntax Syntax { get; }
}

[Closed(
    typeof(ITypeDeclaration))]
public partial interface IClassMemberDeclaration : ITypeMemberDeclaration
{
    new IClassMemberDeclarationSyntax Syntax { get; }
}

[Closed(
    typeof(ITypeDeclaration))]
public partial interface ITraitMemberDeclaration : ITypeMemberDeclaration
{
    new ITraitMemberDeclarationSyntax Syntax { get; }
}

[Closed(
    typeof(ITypeDeclaration))]
public partial interface IStructMemberDeclaration : ITypeMemberDeclaration
{
    new IStructMemberDeclarationSyntax Syntax { get; }
}

[Closed(
    typeof(ICapabilitySet),
    typeof(ICapability))]
public partial interface ICapabilityConstraint : ICode
{
    new ICapabilityConstraintSyntax Syntax { get; }
    ICapabilityConstraint Constraint { get; }
}

public partial interface ICapabilitySet : ISemanticNode, ICapabilityConstraint
{
    new ICapabilitySetSyntax Syntax { get; }
    new CapabilitySet Constraint { get; }
}

public partial interface ICapability : ISemanticNode, ICapabilityConstraint
{
    new ICapabilitySyntax Syntax { get; }
    Capability Capability { get; }
    new Capability Constraint { get; }
}

