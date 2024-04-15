using System.CodeDom.Compiler;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.IST;

// ReSharper disable InconsistentNaming

[GeneratedCode("AzothCompilerCodeGen", null)]
public sealed partial class WithTypeDeclarationPromises
{
    public partial interface Package : IImplementationRestricted
    {
        IFixedSet<NamespaceMemberDeclaration> Declarations { get; }
        IFixedSet<NamespaceMemberDeclaration> TestingDeclarations { get; }
        PackageReferenceScope LexicalScope { get; }
        IPackageSyntax Syntax { get; }
        PackageSymbol Symbol { get; }
        IFixedSet<PackageReference> References { get; }

        public static Package Create(IEnumerable<NamespaceMemberDeclaration> declarations, IEnumerable<NamespaceMemberDeclaration> testingDeclarations, PackageReferenceScope lexicalScope, IPackageSyntax syntax, PackageSymbol symbol, IEnumerable<PackageReference> references)
            => new PackageNode(declarations.ToFixedSet(), testingDeclarations.ToFixedSet(), lexicalScope, syntax, symbol, references.ToFixedSet());
    }

    [Closed(
        typeof(TypeDeclaration),
        typeof(FunctionDeclaration))]
    public partial interface NamespaceMemberDeclaration : Declaration
    {
        CodeFile File { get; }
    }

    [Closed(
        typeof(NamespaceMemberDeclaration),
        typeof(TypeMemberDeclaration))]
    public partial interface Declaration : Code
    {
        DeclarationLexicalScope ContainingLexicalScope { get; }
        NamespaceSymbol? ContainingSymbol { get; }
        new IDeclarationSyntax Syntax { get; }
        ISyntax Code.Syntax => Syntax;
    }

    [Closed(
        typeof(ClassDeclaration),
        typeof(StructDeclaration),
        typeof(TraitDeclaration))]
    public partial interface TypeDeclaration : NamespaceMemberDeclaration, ClassMemberDeclaration, TraitMemberDeclaration, StructMemberDeclaration
    {
        DeclarationScope LexicalScope { get; }
        new ITypeDeclarationSyntax Syntax { get; }
        IDeclarationSyntax Declaration.Syntax => Syntax;
        IClassMemberDeclarationSyntax ClassMemberDeclaration.Syntax => Syntax;
        ITraitMemberDeclarationSyntax TraitMemberDeclaration.Syntax => Syntax;
        IStructMemberDeclarationSyntax StructMemberDeclaration.Syntax => Syntax;
        ISyntax Code.Syntax => Syntax;
        ITypeMemberDeclarationSyntax TypeMemberDeclaration.Syntax => Syntax;
        IFixedList<GenericParameter> GenericParameters { get; }
        IFixedList<UnresolvedSupertypeName> SupertypeNames { get; }
    }

    public partial interface UnresolvedSupertypeName : Code
    {
        DeclarationLexicalScope ContainingLexicalScope { get; }
        new ISupertypeNameSyntax Syntax { get; }
        ISyntax Code.Syntax => Syntax;
        TypeName Name { get; }
        IFixedList<UnresolvedType> TypeArguments { get; }

        public static UnresolvedSupertypeName Create(DeclarationLexicalScope containingLexicalScope, ISupertypeNameSyntax syntax, TypeName name, IEnumerable<UnresolvedType> typeArguments)
            => new UnresolvedSupertypeNameNode(containingLexicalScope, syntax, name, typeArguments.ToFixedList());
    }

    [Closed(
        typeof(UnresolvedStandardTypeName),
        typeof(UnresolvedSimpleTypeName),
        typeof(UnresolvedQualifiedTypeName))]
    public partial interface UnresolvedTypeName : UnresolvedType
    {
        DeclarationLexicalScope ContainingLexicalScope { get; }
        new ITypeNameSyntax Syntax { get; }
        ITypeSyntax UnresolvedType.Syntax => Syntax;
        TypeName Name { get; }
    }

    public partial interface FunctionDeclaration : NamespaceMemberDeclaration
    {
        new NamespaceSymbol ContainingSymbol { get; }
        NamespaceSymbol? Declaration.ContainingSymbol => ContainingSymbol;
        new IFunctionDeclarationSyntax Syntax { get; }
        IDeclarationSyntax Declaration.Syntax => Syntax;

        public static FunctionDeclaration Create(NamespaceSymbol containingSymbol, IFunctionDeclarationSyntax syntax, CodeFile file, DeclarationLexicalScope containingLexicalScope)
            => new FunctionDeclarationNode(containingSymbol, syntax, file, containingLexicalScope);
    }

    public partial interface PackageReference : IImplementationRestricted
    {
        IPackageReferenceSyntax Syntax { get; }
        IdentifierName AliasOrName { get; }
        IPackageSymbols Package { get; }
        bool IsTrusted { get; }

        public static PackageReference Create(IPackageReferenceSyntax syntax, IdentifierName aliasOrName, IPackageSymbols package, bool isTrusted)
            => new PackageReferenceNode(syntax, aliasOrName, package, isTrusted);
    }

    [Closed(
        typeof(Declaration),
        typeof(UnresolvedSupertypeName),
        typeof(GenericParameter),
        typeof(CapabilityConstraint),
        typeof(UnresolvedType))]
    public partial interface Code : IImplementationRestricted
    {
        ISyntax Syntax { get; }
    }

    public partial interface ClassDeclaration : TypeDeclaration
    {
        new IClassDeclarationSyntax Syntax { get; }
        ITypeDeclarationSyntax TypeDeclaration.Syntax => Syntax;
        bool IsAbstract { get; }
        UnresolvedSupertypeName? BaseTypeName { get; }
        IFixedList<ClassMemberDeclaration> Members { get; }

        public static ClassDeclaration Create(IClassDeclarationSyntax syntax, bool isAbstract, UnresolvedSupertypeName? baseTypeName, IEnumerable<ClassMemberDeclaration> members, DeclarationScope lexicalScope, IEnumerable<GenericParameter> genericParameters, IEnumerable<UnresolvedSupertypeName> supertypeNames, CodeFile file, DeclarationLexicalScope containingLexicalScope, NamespaceSymbol? containingSymbol)
            => new ClassDeclarationNode(syntax, isAbstract, baseTypeName, members.ToFixedList(), lexicalScope, genericParameters.ToFixedList(), supertypeNames.ToFixedList(), file, containingLexicalScope, containingSymbol);
    }

    public partial interface StructDeclaration : TypeDeclaration
    {
        new IStructDeclarationSyntax Syntax { get; }
        ITypeDeclarationSyntax TypeDeclaration.Syntax => Syntax;
        IFixedList<StructMemberDeclaration> Members { get; }

        public static StructDeclaration Create(IStructDeclarationSyntax syntax, IEnumerable<StructMemberDeclaration> members, DeclarationScope lexicalScope, IEnumerable<GenericParameter> genericParameters, IEnumerable<UnresolvedSupertypeName> supertypeNames, CodeFile file, DeclarationLexicalScope containingLexicalScope, NamespaceSymbol? containingSymbol)
            => new StructDeclarationNode(syntax, members.ToFixedList(), lexicalScope, genericParameters.ToFixedList(), supertypeNames.ToFixedList(), file, containingLexicalScope, containingSymbol);
    }

    public partial interface TraitDeclaration : TypeDeclaration
    {
        new ITraitDeclarationSyntax Syntax { get; }
        ITypeDeclarationSyntax TypeDeclaration.Syntax => Syntax;
        IFixedList<TraitMemberDeclaration> Members { get; }

        public static TraitDeclaration Create(ITraitDeclarationSyntax syntax, IEnumerable<TraitMemberDeclaration> members, DeclarationScope lexicalScope, IEnumerable<GenericParameter> genericParameters, IEnumerable<UnresolvedSupertypeName> supertypeNames, CodeFile file, DeclarationLexicalScope containingLexicalScope, NamespaceSymbol? containingSymbol)
            => new TraitDeclarationNode(syntax, members.ToFixedList(), lexicalScope, genericParameters.ToFixedList(), supertypeNames.ToFixedList(), file, containingLexicalScope, containingSymbol);
    }

    public partial interface GenericParameter : Code
    {
        new IGenericParameterSyntax Syntax { get; }
        ISyntax Code.Syntax => Syntax;
        CapabilityConstraint Constraint { get; }
        IdentifierName Name { get; }
        ParameterIndependence Independence { get; }
        ParameterVariance Variance { get; }

        public static GenericParameter Create(IGenericParameterSyntax syntax, CapabilityConstraint constraint, IdentifierName name, ParameterIndependence independence, ParameterVariance variance)
            => new GenericParameterNode(syntax, constraint, name, independence, variance);
    }

    [Closed(
        typeof(ClassMemberDeclaration),
        typeof(TraitMemberDeclaration),
        typeof(StructMemberDeclaration))]
    public partial interface TypeMemberDeclaration : Declaration
    {
        new ITypeMemberDeclarationSyntax Syntax { get; }
        IDeclarationSyntax Declaration.Syntax => Syntax;
    }

    [Closed(
        typeof(TypeDeclaration))]
    public partial interface ClassMemberDeclaration : TypeMemberDeclaration
    {
        new IClassMemberDeclarationSyntax Syntax { get; }
        ITypeMemberDeclarationSyntax TypeMemberDeclaration.Syntax => Syntax;
    }

    [Closed(
        typeof(TypeDeclaration))]
    public partial interface TraitMemberDeclaration : TypeMemberDeclaration
    {
        new ITraitMemberDeclarationSyntax Syntax { get; }
        ITypeMemberDeclarationSyntax TypeMemberDeclaration.Syntax => Syntax;
    }

    [Closed(
        typeof(TypeDeclaration))]
    public partial interface StructMemberDeclaration : TypeMemberDeclaration
    {
        new IStructMemberDeclarationSyntax Syntax { get; }
        ITypeMemberDeclarationSyntax TypeMemberDeclaration.Syntax => Syntax;
    }

    [Closed(
        typeof(CapabilitySet),
        typeof(Capability))]
    public partial interface CapabilityConstraint : Code
    {
        new ICapabilityConstraintSyntax Syntax { get; }
        ISyntax Code.Syntax => Syntax;
        ICapabilityConstraint Constraint { get; }
    }

    public partial interface CapabilitySet : CapabilityConstraint
    {
        new ICapabilitySetSyntax Syntax { get; }
        ICapabilityConstraintSyntax CapabilityConstraint.Syntax => Syntax;
        new Types.Capabilities.CapabilitySet Constraint { get; }
        ICapabilityConstraint CapabilityConstraint.Constraint => Constraint;

        public static CapabilitySet Create(ICapabilitySetSyntax syntax, Types.Capabilities.CapabilitySet constraint)
            => new CapabilitySetNode(syntax, constraint);
    }

    public partial interface Capability : CapabilityConstraint
    {
        new ICapabilitySyntax Syntax { get; }
        ICapabilityConstraintSyntax CapabilityConstraint.Syntax => Syntax;
        Types.Capabilities.Capability Capability { get; }
        new Types.Capabilities.Capability Constraint { get; }
        ICapabilityConstraint CapabilityConstraint.Constraint => Constraint;

        public static Capability Create(ICapabilitySyntax syntax, Types.Capabilities.Capability capability, Types.Capabilities.Capability constraint)
            => new CapabilityNode(syntax, capability, constraint);
    }

    [Closed(
        typeof(UnresolvedTypeName),
        typeof(UnresolvedOptionalType),
        typeof(UnresolvedCapabilityType),
        typeof(UnresolvedFunctionType),
        typeof(UnresolvedViewpointType))]
    public partial interface UnresolvedType : Code
    {
        new ITypeSyntax Syntax { get; }
        ISyntax Code.Syntax => Syntax;
    }

    [Closed(
        typeof(UnresolvedIdentifierTypeName),
        typeof(UnresolvedGenericTypeName))]
    public partial interface UnresolvedStandardTypeName : UnresolvedTypeName
    {
        new IStandardTypeNameSyntax Syntax { get; }
        ITypeNameSyntax UnresolvedTypeName.Syntax => Syntax;
        new StandardName Name { get; }
        TypeName UnresolvedTypeName.Name => Name;
    }

    [Closed(
        typeof(UnresolvedIdentifierTypeName),
        typeof(UnresolvedSpecialTypeName))]
    public partial interface UnresolvedSimpleTypeName : UnresolvedTypeName
    {
        new ISimpleTypeNameSyntax Syntax { get; }
        ITypeNameSyntax UnresolvedTypeName.Syntax => Syntax;
    }

    public partial interface UnresolvedIdentifierTypeName : UnresolvedStandardTypeName, UnresolvedSimpleTypeName
    {
        new IIdentifierTypeNameSyntax Syntax { get; }
        IStandardTypeNameSyntax UnresolvedStandardTypeName.Syntax => Syntax;
        ISimpleTypeNameSyntax UnresolvedSimpleTypeName.Syntax => Syntax;
        ITypeNameSyntax UnresolvedTypeName.Syntax => Syntax;
        new IdentifierName Name { get; }
        StandardName UnresolvedStandardTypeName.Name => Name;
        TypeName UnresolvedTypeName.Name => Name;

        public static UnresolvedIdentifierTypeName Create(IIdentifierTypeNameSyntax syntax, IdentifierName name, DeclarationLexicalScope containingLexicalScope)
            => new UnresolvedIdentifierTypeNameNode(syntax, name, containingLexicalScope);
    }

    public partial interface UnresolvedSpecialTypeName : UnresolvedSimpleTypeName
    {
        new ISpecialTypeNameSyntax Syntax { get; }
        ISimpleTypeNameSyntax UnresolvedSimpleTypeName.Syntax => Syntax;
        new SpecialTypeName Name { get; }
        TypeName UnresolvedTypeName.Name => Name;

        public static UnresolvedSpecialTypeName Create(ISpecialTypeNameSyntax syntax, SpecialTypeName name, DeclarationLexicalScope containingLexicalScope)
            => new UnresolvedSpecialTypeNameNode(syntax, name, containingLexicalScope);
    }

    public partial interface UnresolvedGenericTypeName : UnresolvedStandardTypeName
    {
        new IGenericTypeNameSyntax Syntax { get; }
        IStandardTypeNameSyntax UnresolvedStandardTypeName.Syntax => Syntax;
        new GenericName Name { get; }
        StandardName UnresolvedStandardTypeName.Name => Name;
        IFixedList<UnresolvedType> TypeArguments { get; }

        public static UnresolvedGenericTypeName Create(IGenericTypeNameSyntax syntax, GenericName name, IEnumerable<UnresolvedType> typeArguments, DeclarationLexicalScope containingLexicalScope)
            => new UnresolvedGenericTypeNameNode(syntax, name, typeArguments.ToFixedList(), containingLexicalScope);
    }

    public partial interface UnresolvedQualifiedTypeName : UnresolvedTypeName
    {
        new IQualifiedTypeNameSyntax Syntax { get; }
        ITypeNameSyntax UnresolvedTypeName.Syntax => Syntax;
        UnresolvedTypeName Context { get; }
        UnresolvedStandardTypeName QualifiedName { get; }

        public static UnresolvedQualifiedTypeName Create(IQualifiedTypeNameSyntax syntax, UnresolvedTypeName context, UnresolvedStandardTypeName qualifiedName, DeclarationLexicalScope containingLexicalScope, TypeName name)
            => new UnresolvedQualifiedTypeNameNode(syntax, context, qualifiedName, containingLexicalScope, name);
    }

    public partial interface UnresolvedOptionalType : UnresolvedType
    {
        new IOptionalTypeSyntax Syntax { get; }
        ITypeSyntax UnresolvedType.Syntax => Syntax;
        UnresolvedType Referent { get; }

        public static UnresolvedOptionalType Create(IOptionalTypeSyntax syntax, UnresolvedType referent)
            => new UnresolvedOptionalTypeNode(syntax, referent);
    }

    public partial interface UnresolvedCapabilityType : UnresolvedType
    {
        new ICapabilityTypeSyntax Syntax { get; }
        ITypeSyntax UnresolvedType.Syntax => Syntax;
        Capability Capability { get; }
        UnresolvedType Referent { get; }

        public static UnresolvedCapabilityType Create(ICapabilityTypeSyntax syntax, Capability capability, UnresolvedType referent)
            => new UnresolvedCapabilityTypeNode(syntax, capability, referent);
    }

    public partial interface UnresolvedFunctionType : UnresolvedType
    {
        new IFunctionTypeSyntax Syntax { get; }
        ITypeSyntax UnresolvedType.Syntax => Syntax;
        IFixedList<UnresolvedParameterType> Parameters { get; }
        UnresolvedType Return { get; }

        public static UnresolvedFunctionType Create(IFunctionTypeSyntax syntax, IEnumerable<UnresolvedParameterType> parameters, UnresolvedType @return)
            => new UnresolvedFunctionTypeNode(syntax, parameters.ToFixedList(), @return);
    }

    public partial interface UnresolvedParameterType : IImplementationRestricted
    {
        IParameterTypeSyntax Syntax { get; }
        bool IsLent { get; }
        UnresolvedType Referent { get; }

        public static UnresolvedParameterType Create(IParameterTypeSyntax syntax, bool isLent, UnresolvedType referent)
            => new UnresolvedParameterTypeNode(syntax, isLent, referent);
    }

    [Closed(
        typeof(UnresolvedCapabilityViewpointType),
        typeof(UnresolvedSelfViewpointType))]
    public partial interface UnresolvedViewpointType : UnresolvedType
    {
        new IViewpointTypeSyntax Syntax { get; }
        ITypeSyntax UnresolvedType.Syntax => Syntax;
        UnresolvedType Referent { get; }
    }

    public partial interface UnresolvedCapabilityViewpointType : UnresolvedViewpointType
    {
        new ICapabilityViewpointTypeSyntax Syntax { get; }
        IViewpointTypeSyntax UnresolvedViewpointType.Syntax => Syntax;
        Capability Capability { get; }

        public static UnresolvedCapabilityViewpointType Create(ICapabilityViewpointTypeSyntax syntax, Capability capability, UnresolvedType referent)
            => new UnresolvedCapabilityViewpointTypeNode(syntax, capability, referent);
    }

    public partial interface UnresolvedSelfViewpointType : UnresolvedViewpointType
    {
        new ISelfViewpointTypeSyntax Syntax { get; }
        IViewpointTypeSyntax UnresolvedViewpointType.Syntax => Syntax;

        public static UnresolvedSelfViewpointType Create(ISelfViewpointTypeSyntax syntax, UnresolvedType referent)
            => new UnresolvedSelfViewpointTypeNode(syntax, referent);
    }

}

public sealed partial class WithoutCompilationUnits
{
    public partial interface Package : WithTypeDeclarationPromises.Package
    {
        IFixedSet<WithTypeDeclarationPromises.NamespaceMemberDeclaration> WithTypeDeclarationPromises.Package.Declarations => Declarations;
        IFixedSet<WithTypeDeclarationPromises.NamespaceMemberDeclaration> WithTypeDeclarationPromises.Package.TestingDeclarations => TestingDeclarations;
        IFixedSet<WithTypeDeclarationPromises.PackageReference> WithTypeDeclarationPromises.Package.References => References;
    }

    public partial interface NamespaceMemberDeclaration : WithTypeDeclarationPromises.NamespaceMemberDeclaration
    {
    }

    public partial interface Declaration : WithTypeDeclarationPromises.Declaration
    {
    }

    public partial interface TypeDeclaration : WithTypeDeclarationPromises.TypeDeclaration
    {
        IFixedList<WithTypeDeclarationPromises.GenericParameter> WithTypeDeclarationPromises.TypeDeclaration.GenericParameters => GenericParameters;
        IFixedList<WithTypeDeclarationPromises.UnresolvedSupertypeName> WithTypeDeclarationPromises.TypeDeclaration.SupertypeNames => SupertypeNames;
    }

    public partial interface UnresolvedSupertypeName : WithTypeDeclarationPromises.UnresolvedSupertypeName
    {
        IFixedList<WithTypeDeclarationPromises.UnresolvedType> WithTypeDeclarationPromises.UnresolvedSupertypeName.TypeArguments => TypeArguments;
    }

    public partial interface UnresolvedTypeName : WithTypeDeclarationPromises.UnresolvedTypeName
    {
    }

    public partial interface FunctionDeclaration : WithTypeDeclarationPromises.FunctionDeclaration
    {
    }

    public partial interface PackageReference : WithTypeDeclarationPromises.PackageReference
    {
    }

    public partial interface Code : WithTypeDeclarationPromises.Code
    {
    }

    public partial interface ClassDeclaration : WithTypeDeclarationPromises.ClassDeclaration
    {
        WithTypeDeclarationPromises.UnresolvedSupertypeName? WithTypeDeclarationPromises.ClassDeclaration.BaseTypeName => BaseTypeName;
        IFixedList<WithTypeDeclarationPromises.ClassMemberDeclaration> WithTypeDeclarationPromises.ClassDeclaration.Members => Members;
    }

    public partial interface StructDeclaration : WithTypeDeclarationPromises.StructDeclaration
    {
        IFixedList<WithTypeDeclarationPromises.StructMemberDeclaration> WithTypeDeclarationPromises.StructDeclaration.Members => Members;
    }

    public partial interface TraitDeclaration : WithTypeDeclarationPromises.TraitDeclaration
    {
        IFixedList<WithTypeDeclarationPromises.TraitMemberDeclaration> WithTypeDeclarationPromises.TraitDeclaration.Members => Members;
    }

    public partial interface GenericParameter : WithTypeDeclarationPromises.GenericParameter
    {
        WithTypeDeclarationPromises.CapabilityConstraint WithTypeDeclarationPromises.GenericParameter.Constraint => Constraint;
    }

    public partial interface TypeMemberDeclaration : WithTypeDeclarationPromises.TypeMemberDeclaration
    {
    }

    public partial interface ClassMemberDeclaration : WithTypeDeclarationPromises.ClassMemberDeclaration
    {
    }

    public partial interface TraitMemberDeclaration : WithTypeDeclarationPromises.TraitMemberDeclaration
    {
    }

    public partial interface StructMemberDeclaration : WithTypeDeclarationPromises.StructMemberDeclaration
    {
    }

    public partial interface CapabilityConstraint : WithTypeDeclarationPromises.CapabilityConstraint
    {
    }

    public partial interface CapabilitySet : WithTypeDeclarationPromises.CapabilitySet
    {
    }

    public partial interface Capability : WithTypeDeclarationPromises.Capability
    {
    }

    public partial interface UnresolvedType : WithTypeDeclarationPromises.UnresolvedType
    {
    }

    public partial interface UnresolvedStandardTypeName : WithTypeDeclarationPromises.UnresolvedStandardTypeName
    {
    }

    public partial interface UnresolvedSimpleTypeName : WithTypeDeclarationPromises.UnresolvedSimpleTypeName
    {
    }

    public partial interface UnresolvedIdentifierTypeName : WithTypeDeclarationPromises.UnresolvedIdentifierTypeName
    {
    }

    public partial interface UnresolvedSpecialTypeName : WithTypeDeclarationPromises.UnresolvedSpecialTypeName
    {
    }

    public partial interface UnresolvedGenericTypeName : WithTypeDeclarationPromises.UnresolvedGenericTypeName
    {
        IFixedList<WithTypeDeclarationPromises.UnresolvedType> WithTypeDeclarationPromises.UnresolvedGenericTypeName.TypeArguments => TypeArguments;
    }

    public partial interface UnresolvedQualifiedTypeName : WithTypeDeclarationPromises.UnresolvedQualifiedTypeName
    {
        WithTypeDeclarationPromises.UnresolvedTypeName WithTypeDeclarationPromises.UnresolvedQualifiedTypeName.Context => Context;
        WithTypeDeclarationPromises.UnresolvedStandardTypeName WithTypeDeclarationPromises.UnresolvedQualifiedTypeName.QualifiedName => QualifiedName;
    }

    public partial interface UnresolvedOptionalType : WithTypeDeclarationPromises.UnresolvedOptionalType
    {
        WithTypeDeclarationPromises.UnresolvedType WithTypeDeclarationPromises.UnresolvedOptionalType.Referent => Referent;
    }

    public partial interface UnresolvedCapabilityType : WithTypeDeclarationPromises.UnresolvedCapabilityType
    {
        WithTypeDeclarationPromises.Capability WithTypeDeclarationPromises.UnresolvedCapabilityType.Capability => Capability;
        WithTypeDeclarationPromises.UnresolvedType WithTypeDeclarationPromises.UnresolvedCapabilityType.Referent => Referent;
    }

    public partial interface UnresolvedFunctionType : WithTypeDeclarationPromises.UnresolvedFunctionType
    {
        IFixedList<WithTypeDeclarationPromises.UnresolvedParameterType> WithTypeDeclarationPromises.UnresolvedFunctionType.Parameters => Parameters;
        WithTypeDeclarationPromises.UnresolvedType WithTypeDeclarationPromises.UnresolvedFunctionType.Return => Return;
    }

    public partial interface UnresolvedParameterType : WithTypeDeclarationPromises.UnresolvedParameterType
    {
        WithTypeDeclarationPromises.UnresolvedType WithTypeDeclarationPromises.UnresolvedParameterType.Referent => Referent;
    }

    public partial interface UnresolvedViewpointType : WithTypeDeclarationPromises.UnresolvedViewpointType
    {
        WithTypeDeclarationPromises.UnresolvedType WithTypeDeclarationPromises.UnresolvedViewpointType.Referent => Referent;
    }

    public partial interface UnresolvedCapabilityViewpointType : WithTypeDeclarationPromises.UnresolvedCapabilityViewpointType
    {
        WithTypeDeclarationPromises.Capability WithTypeDeclarationPromises.UnresolvedCapabilityViewpointType.Capability => Capability;
    }

    public partial interface UnresolvedSelfViewpointType : WithTypeDeclarationPromises.UnresolvedSelfViewpointType
    {
    }

}

