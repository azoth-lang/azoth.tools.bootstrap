using System.CodeDom.Compiler;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.IST;

// ReSharper disable InconsistentNaming

[GeneratedCode("AzothCompilerCodeGen", null)]
public sealed partial class WithDeclarationLexicalScopes
{
    [Closed(
        typeof(NamespaceMemberDeclaration),
        typeof(TypeMemberDeclaration))]
    public partial interface Declaration : Code
    {
        NamespaceSymbol? ContainingSymbol { get; }
        new IDeclarationSyntax Syntax { get; }
        ISyntax Code.Syntax => Syntax;
    }

    public partial interface NamespaceDeclaration : NamespaceMemberDeclaration
    {
        new NamespaceSymbol ContainingSymbol { get; }
        NamespaceSymbol? Declaration.ContainingSymbol => ContainingSymbol;
        NamespaceSymbol Symbol { get; }
        new INamespaceDeclarationSyntax Syntax { get; }
        IDeclarationSyntax Declaration.Syntax => Syntax;
        bool IsGlobalQualified { get; }
        NamespaceName DeclaredNames { get; }
        IFixedList<UsingDirective> UsingDirectives { get; }
        IFixedList<NamespaceMemberDeclaration> Declarations { get; }

        public static NamespaceDeclaration Create(NamespaceSymbol containingSymbol, NamespaceSymbol symbol, INamespaceDeclarationSyntax syntax, bool isGlobalQualified, NamespaceName declaredNames, IEnumerable<UsingDirective> usingDirectives, IEnumerable<NamespaceMemberDeclaration> declarations)
            => new NamespaceDeclarationNode(containingSymbol, symbol, syntax, isGlobalQualified, declaredNames, usingDirectives.ToFixedList(), declarations.ToFixedList());
    }

    public partial interface FunctionDeclaration : NamespaceMemberDeclaration
    {
        new NamespaceSymbol ContainingSymbol { get; }
        NamespaceSymbol? Declaration.ContainingSymbol => ContainingSymbol;
        new IFunctionDeclarationSyntax Syntax { get; }
        IDeclarationSyntax Declaration.Syntax => Syntax;

        public static FunctionDeclaration Create(NamespaceSymbol containingSymbol, IFunctionDeclarationSyntax syntax)
            => new FunctionDeclarationNode(containingSymbol, syntax);
    }

    public partial interface Package : IImplementationRestricted
    {
        IPackageSyntax Syntax { get; }
        PackageSymbol Symbol { get; }
        IFixedSet<PackageReference> References { get; }
        IFixedSet<CompilationUnit> CompilationUnits { get; }
        IFixedSet<CompilationUnit> TestingCompilationUnits { get; }

        public static Package Create(IPackageSyntax syntax, PackageSymbol symbol, IEnumerable<PackageReference> references, IEnumerable<CompilationUnit> compilationUnits, IEnumerable<CompilationUnit> testingCompilationUnits)
            => new PackageNode(syntax, symbol, references.ToFixedSet(), compilationUnits.ToFixedSet(), testingCompilationUnits.ToFixedSet());
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

    public partial interface CompilationUnit : Code
    {
        new ICompilationUnitSyntax Syntax { get; }
        ISyntax Code.Syntax => Syntax;
        CodeFile File { get; }
        NamespaceName ImplicitNamespaceName { get; }
        IFixedList<UsingDirective> UsingDirectives { get; }
        IFixedList<NamespaceMemberDeclaration> Declarations { get; }

        public static CompilationUnit Create(ICompilationUnitSyntax syntax, CodeFile file, NamespaceName implicitNamespaceName, IEnumerable<UsingDirective> usingDirectives, IEnumerable<NamespaceMemberDeclaration> declarations)
            => new CompilationUnitNode(syntax, file, implicitNamespaceName, usingDirectives.ToFixedList(), declarations.ToFixedList());
    }

    public partial interface UsingDirective : Code
    {
        new IUsingDirectiveSyntax Syntax { get; }
        ISyntax Code.Syntax => Syntax;
        NamespaceName Name { get; }

        public static UsingDirective Create(IUsingDirectiveSyntax syntax, NamespaceName name)
            => new UsingDirectiveNode(syntax, name);
    }

    [Closed(
        typeof(Declaration),
        typeof(CompilationUnit),
        typeof(UsingDirective),
        typeof(GenericParameter),
        typeof(UnresolvedSupertypeName),
        typeof(CapabilityConstraint),
        typeof(UnresolvedType))]
    public partial interface Code : IImplementationRestricted
    {
        ISyntax Syntax { get; }
    }

    [Closed(
        typeof(NamespaceDeclaration),
        typeof(FunctionDeclaration),
        typeof(TypeDeclaration))]
    public partial interface NamespaceMemberDeclaration : Declaration
    {
    }

    [Closed(
        typeof(ClassDeclaration),
        typeof(StructDeclaration),
        typeof(TraitDeclaration))]
    public partial interface TypeDeclaration : NamespaceMemberDeclaration, ClassMemberDeclaration, TraitMemberDeclaration, StructMemberDeclaration
    {
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

    public partial interface ClassDeclaration : TypeDeclaration
    {
        new IClassDeclarationSyntax Syntax { get; }
        ITypeDeclarationSyntax TypeDeclaration.Syntax => Syntax;
        bool IsAbstract { get; }
        UnresolvedSupertypeName? BaseTypeName { get; }
        IFixedList<ClassMemberDeclaration> Members { get; }

        public static ClassDeclaration Create(IClassDeclarationSyntax syntax, bool isAbstract, UnresolvedSupertypeName? baseTypeName, IEnumerable<ClassMemberDeclaration> members, IEnumerable<GenericParameter> genericParameters, IEnumerable<UnresolvedSupertypeName> supertypeNames, NamespaceSymbol? containingSymbol)
            => new ClassDeclarationNode(syntax, isAbstract, baseTypeName, members.ToFixedList(), genericParameters.ToFixedList(), supertypeNames.ToFixedList(), containingSymbol);
    }

    public partial interface StructDeclaration : TypeDeclaration
    {
        new IStructDeclarationSyntax Syntax { get; }
        ITypeDeclarationSyntax TypeDeclaration.Syntax => Syntax;
        IFixedList<StructMemberDeclaration> Members { get; }

        public static StructDeclaration Create(IStructDeclarationSyntax syntax, IEnumerable<StructMemberDeclaration> members, IEnumerable<GenericParameter> genericParameters, IEnumerable<UnresolvedSupertypeName> supertypeNames, NamespaceSymbol? containingSymbol)
            => new StructDeclarationNode(syntax, members.ToFixedList(), genericParameters.ToFixedList(), supertypeNames.ToFixedList(), containingSymbol);
    }

    public partial interface TraitDeclaration : TypeDeclaration
    {
        new ITraitDeclarationSyntax Syntax { get; }
        ITypeDeclarationSyntax TypeDeclaration.Syntax => Syntax;
        IFixedList<TraitMemberDeclaration> Members { get; }

        public static TraitDeclaration Create(ITraitDeclarationSyntax syntax, IEnumerable<TraitMemberDeclaration> members, IEnumerable<GenericParameter> genericParameters, IEnumerable<UnresolvedSupertypeName> supertypeNames, NamespaceSymbol? containingSymbol)
            => new TraitDeclarationNode(syntax, members.ToFixedList(), genericParameters.ToFixedList(), supertypeNames.ToFixedList(), containingSymbol);
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

    public partial interface UnresolvedSupertypeName : Code
    {
        new ISupertypeNameSyntax Syntax { get; }
        ISyntax Code.Syntax => Syntax;
        TypeName Name { get; }
        IFixedList<UnresolvedType> TypeArguments { get; }

        public static UnresolvedSupertypeName Create(ISupertypeNameSyntax syntax, TypeName name, IEnumerable<UnresolvedType> typeArguments)
            => new UnresolvedSupertypeNameNode(syntax, name, typeArguments.ToFixedList());
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
        typeof(UnresolvedStandardTypeName),
        typeof(UnresolvedSimpleTypeName),
        typeof(UnresolvedQualifiedTypeName))]
    public partial interface UnresolvedTypeName : UnresolvedType
    {
        new ITypeNameSyntax Syntax { get; }
        ITypeSyntax UnresolvedType.Syntax => Syntax;
        TypeName Name { get; }
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

        public static UnresolvedIdentifierTypeName Create(IIdentifierTypeNameSyntax syntax, IdentifierName name)
            => new UnresolvedIdentifierTypeNameNode(syntax, name);
    }

    public partial interface UnresolvedSpecialTypeName : UnresolvedSimpleTypeName
    {
        new ISpecialTypeNameSyntax Syntax { get; }
        ISimpleTypeNameSyntax UnresolvedSimpleTypeName.Syntax => Syntax;
        new SpecialTypeName Name { get; }
        TypeName UnresolvedTypeName.Name => Name;

        public static UnresolvedSpecialTypeName Create(ISpecialTypeNameSyntax syntax, SpecialTypeName name)
            => new UnresolvedSpecialTypeNameNode(syntax, name);
    }

    public partial interface UnresolvedGenericTypeName : UnresolvedStandardTypeName
    {
        new IGenericTypeNameSyntax Syntax { get; }
        IStandardTypeNameSyntax UnresolvedStandardTypeName.Syntax => Syntax;
        new GenericName Name { get; }
        StandardName UnresolvedStandardTypeName.Name => Name;
        IFixedList<UnresolvedType> TypeArguments { get; }

        public static UnresolvedGenericTypeName Create(IGenericTypeNameSyntax syntax, GenericName name, IEnumerable<UnresolvedType> typeArguments)
            => new UnresolvedGenericTypeNameNode(syntax, name, typeArguments.ToFixedList());
    }

    public partial interface UnresolvedQualifiedTypeName : UnresolvedTypeName
    {
        new IQualifiedTypeNameSyntax Syntax { get; }
        ITypeNameSyntax UnresolvedTypeName.Syntax => Syntax;
        UnresolvedTypeName Context { get; }
        UnresolvedStandardTypeName QualifiedName { get; }

        public static UnresolvedQualifiedTypeName Create(IQualifiedTypeNameSyntax syntax, UnresolvedTypeName context, UnresolvedStandardTypeName qualifiedName, TypeName name)
            => new UnresolvedQualifiedTypeNameNode(syntax, context, qualifiedName, name);
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

public sealed partial class WithNamespaceSymbols
{
    public partial interface Declaration : WithDeclarationLexicalScopes.Declaration
    {
    }

    public partial interface NamespaceDeclaration : WithDeclarationLexicalScopes.NamespaceDeclaration
    {
        IFixedList<WithDeclarationLexicalScopes.UsingDirective> WithDeclarationLexicalScopes.NamespaceDeclaration.UsingDirectives => UsingDirectives;
        IFixedList<WithDeclarationLexicalScopes.NamespaceMemberDeclaration> WithDeclarationLexicalScopes.NamespaceDeclaration.Declarations => Declarations;
    }

    public partial interface FunctionDeclaration : WithDeclarationLexicalScopes.FunctionDeclaration
    {
    }

    public partial interface Package : WithDeclarationLexicalScopes.Package
    {
        IFixedSet<WithDeclarationLexicalScopes.PackageReference> WithDeclarationLexicalScopes.Package.References => References;
        IFixedSet<WithDeclarationLexicalScopes.CompilationUnit> WithDeclarationLexicalScopes.Package.CompilationUnits => CompilationUnits;
        IFixedSet<WithDeclarationLexicalScopes.CompilationUnit> WithDeclarationLexicalScopes.Package.TestingCompilationUnits => TestingCompilationUnits;
    }

    public partial interface PackageReference : WithDeclarationLexicalScopes.PackageReference
    {
    }

    public partial interface CompilationUnit : WithDeclarationLexicalScopes.CompilationUnit
    {
        IFixedList<WithDeclarationLexicalScopes.UsingDirective> WithDeclarationLexicalScopes.CompilationUnit.UsingDirectives => UsingDirectives;
        IFixedList<WithDeclarationLexicalScopes.NamespaceMemberDeclaration> WithDeclarationLexicalScopes.CompilationUnit.Declarations => Declarations;
    }

    public partial interface UsingDirective : WithDeclarationLexicalScopes.UsingDirective
    {
    }

    public partial interface Code : WithDeclarationLexicalScopes.Code
    {
    }

    public partial interface NamespaceMemberDeclaration : WithDeclarationLexicalScopes.NamespaceMemberDeclaration
    {
    }

    public partial interface TypeDeclaration : WithDeclarationLexicalScopes.TypeDeclaration
    {
        IFixedList<WithDeclarationLexicalScopes.GenericParameter> WithDeclarationLexicalScopes.TypeDeclaration.GenericParameters => GenericParameters;
        IFixedList<WithDeclarationLexicalScopes.UnresolvedSupertypeName> WithDeclarationLexicalScopes.TypeDeclaration.SupertypeNames => SupertypeNames;
    }

    public partial interface ClassDeclaration : WithDeclarationLexicalScopes.ClassDeclaration
    {
        WithDeclarationLexicalScopes.UnresolvedSupertypeName? WithDeclarationLexicalScopes.ClassDeclaration.BaseTypeName => BaseTypeName;
        IFixedList<WithDeclarationLexicalScopes.ClassMemberDeclaration> WithDeclarationLexicalScopes.ClassDeclaration.Members => Members;
    }

    public partial interface StructDeclaration : WithDeclarationLexicalScopes.StructDeclaration
    {
        IFixedList<WithDeclarationLexicalScopes.StructMemberDeclaration> WithDeclarationLexicalScopes.StructDeclaration.Members => Members;
    }

    public partial interface TraitDeclaration : WithDeclarationLexicalScopes.TraitDeclaration
    {
        IFixedList<WithDeclarationLexicalScopes.TraitMemberDeclaration> WithDeclarationLexicalScopes.TraitDeclaration.Members => Members;
    }

    public partial interface GenericParameter : WithDeclarationLexicalScopes.GenericParameter
    {
        WithDeclarationLexicalScopes.CapabilityConstraint WithDeclarationLexicalScopes.GenericParameter.Constraint => Constraint;
    }

    public partial interface UnresolvedSupertypeName : WithDeclarationLexicalScopes.UnresolvedSupertypeName
    {
        IFixedList<WithDeclarationLexicalScopes.UnresolvedType> WithDeclarationLexicalScopes.UnresolvedSupertypeName.TypeArguments => TypeArguments;
    }

    public partial interface TypeMemberDeclaration : WithDeclarationLexicalScopes.TypeMemberDeclaration
    {
    }

    public partial interface ClassMemberDeclaration : WithDeclarationLexicalScopes.ClassMemberDeclaration
    {
    }

    public partial interface TraitMemberDeclaration : WithDeclarationLexicalScopes.TraitMemberDeclaration
    {
    }

    public partial interface StructMemberDeclaration : WithDeclarationLexicalScopes.StructMemberDeclaration
    {
    }

    public partial interface CapabilityConstraint : WithDeclarationLexicalScopes.CapabilityConstraint
    {
    }

    public partial interface CapabilitySet : WithDeclarationLexicalScopes.CapabilitySet
    {
    }

    public partial interface Capability : WithDeclarationLexicalScopes.Capability
    {
    }

    public partial interface UnresolvedType : WithDeclarationLexicalScopes.UnresolvedType
    {
    }

    public partial interface UnresolvedTypeName : WithDeclarationLexicalScopes.UnresolvedTypeName
    {
    }

    public partial interface UnresolvedStandardTypeName : WithDeclarationLexicalScopes.UnresolvedStandardTypeName
    {
    }

    public partial interface UnresolvedSimpleTypeName : WithDeclarationLexicalScopes.UnresolvedSimpleTypeName
    {
    }

    public partial interface UnresolvedIdentifierTypeName : WithDeclarationLexicalScopes.UnresolvedIdentifierTypeName
    {
    }

    public partial interface UnresolvedSpecialTypeName : WithDeclarationLexicalScopes.UnresolvedSpecialTypeName
    {
    }

    public partial interface UnresolvedGenericTypeName : WithDeclarationLexicalScopes.UnresolvedGenericTypeName
    {
        IFixedList<WithDeclarationLexicalScopes.UnresolvedType> WithDeclarationLexicalScopes.UnresolvedGenericTypeName.TypeArguments => TypeArguments;
    }

    public partial interface UnresolvedQualifiedTypeName : WithDeclarationLexicalScopes.UnresolvedQualifiedTypeName
    {
        WithDeclarationLexicalScopes.UnresolvedTypeName WithDeclarationLexicalScopes.UnresolvedQualifiedTypeName.Context => Context;
        WithDeclarationLexicalScopes.UnresolvedStandardTypeName WithDeclarationLexicalScopes.UnresolvedQualifiedTypeName.QualifiedName => QualifiedName;
    }

    public partial interface UnresolvedOptionalType : WithDeclarationLexicalScopes.UnresolvedOptionalType
    {
        WithDeclarationLexicalScopes.UnresolvedType WithDeclarationLexicalScopes.UnresolvedOptionalType.Referent => Referent;
    }

    public partial interface UnresolvedCapabilityType : WithDeclarationLexicalScopes.UnresolvedCapabilityType
    {
        WithDeclarationLexicalScopes.Capability WithDeclarationLexicalScopes.UnresolvedCapabilityType.Capability => Capability;
        WithDeclarationLexicalScopes.UnresolvedType WithDeclarationLexicalScopes.UnresolvedCapabilityType.Referent => Referent;
    }

    public partial interface UnresolvedFunctionType : WithDeclarationLexicalScopes.UnresolvedFunctionType
    {
        IFixedList<WithDeclarationLexicalScopes.UnresolvedParameterType> WithDeclarationLexicalScopes.UnresolvedFunctionType.Parameters => Parameters;
        WithDeclarationLexicalScopes.UnresolvedType WithDeclarationLexicalScopes.UnresolvedFunctionType.Return => Return;
    }

    public partial interface UnresolvedParameterType : WithDeclarationLexicalScopes.UnresolvedParameterType
    {
        WithDeclarationLexicalScopes.UnresolvedType WithDeclarationLexicalScopes.UnresolvedParameterType.Referent => Referent;
    }

    public partial interface UnresolvedViewpointType : WithDeclarationLexicalScopes.UnresolvedViewpointType
    {
        WithDeclarationLexicalScopes.UnresolvedType WithDeclarationLexicalScopes.UnresolvedViewpointType.Referent => Referent;
    }

    public partial interface UnresolvedCapabilityViewpointType : WithDeclarationLexicalScopes.UnresolvedCapabilityViewpointType
    {
        WithDeclarationLexicalScopes.Capability WithDeclarationLexicalScopes.UnresolvedCapabilityViewpointType.Capability => Capability;
    }

    public partial interface UnresolvedSelfViewpointType : WithDeclarationLexicalScopes.UnresolvedSelfViewpointType
    {
    }

}

