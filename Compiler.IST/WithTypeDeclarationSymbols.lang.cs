using System.CodeDom.Compiler;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
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
public sealed partial class WithTypeDeclarationSymbols
{
    [Closed(
        typeof(ClassDeclaration),
        typeof(StructDeclaration),
        typeof(TraitDeclaration))]
    public partial interface TypeDeclaration : NamespaceMemberDeclaration, ClassMemberDeclaration, TraitMemberDeclaration, StructMemberDeclaration
    {
        Symbol ContainingSymbol { get; }
        UserTypeSymbol Symbol { get; }
        DeclarationScope NewScope { get; }
        new ITypeDeclarationSyntax Syntax { get; }
        IDeclarationSyntax Declaration.Syntax => Syntax;
        IClassMemberDeclarationSyntax ClassMemberDeclaration.Syntax => Syntax;
        ITraitMemberDeclarationSyntax TraitMemberDeclaration.Syntax => Syntax;
        IStructMemberDeclarationSyntax StructMemberDeclaration.Syntax => Syntax;
        IConcreteSyntax Code.Syntax => Syntax;
        ITypeMemberDeclarationSyntax TypeMemberDeclaration.Syntax => Syntax;
        IFixedList<GenericParameter> GenericParameters { get; }
        IFixedList<UnresolvedSupertypeName> SupertypeNames { get; }
        IFixedList<TypeMemberDeclaration> Members { get; }
    }

    [Closed(
        typeof(NamespaceMemberDeclaration),
        typeof(TypeMemberDeclaration))]
    public partial interface Declaration : Code
    {
        DeclarationLexicalScope ContainingScope { get; }
        new IDeclarationSyntax Syntax { get; }
        IConcreteSyntax Code.Syntax => Syntax;
    }

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

    public partial interface UnresolvedSupertypeName : Code
    {
        DeclarationLexicalScope ContainingScope { get; }
        new ISupertypeNameSyntax Syntax { get; }
        IConcreteSyntax Code.Syntax => Syntax;
        TypeName Name { get; }
        IFixedList<UnresolvedType> TypeArguments { get; }

        public static UnresolvedSupertypeName Create(DeclarationLexicalScope containingScope, ISupertypeNameSyntax syntax, TypeName name, IEnumerable<UnresolvedType> typeArguments)
            => new UnresolvedSupertypeNameNode(containingScope, syntax, name, typeArguments.ToFixedList());
    }

    [Closed(
        typeof(UnresolvedStandardTypeName),
        typeof(UnresolvedSimpleTypeName),
        typeof(UnresolvedQualifiedTypeName))]
    public partial interface UnresolvedTypeName : UnresolvedType
    {
        DeclarationLexicalScope ContainingScope { get; }
        new ITypeNameSyntax Syntax { get; }
        ITypeSyntax UnresolvedType.Syntax => Syntax;
        TypeName Name { get; }
    }

    public partial interface FunctionDeclaration : NamespaceMemberDeclaration
    {
        NamespaceSymbol ContainingNamespace { get; }
        new IFunctionDeclarationSyntax Syntax { get; }
        IDeclarationSyntax Declaration.Syntax => Syntax;

        public static FunctionDeclaration Create(NamespaceSymbol containingNamespace, IFunctionDeclarationSyntax syntax, CodeFile file, DeclarationLexicalScope containingScope)
            => new FunctionDeclarationNode(containingNamespace, syntax, file, containingScope);
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
        IConcreteSyntax Syntax { get; }
    }

    public partial interface ClassDeclaration : TypeDeclaration
    {
        new IClassDeclarationSyntax Syntax { get; }
        ITypeDeclarationSyntax TypeDeclaration.Syntax => Syntax;
        bool IsAbstract { get; }
        UnresolvedSupertypeName? BaseTypeName { get; }
        new IFixedList<ClassMemberDeclaration> Members { get; }
        IFixedList<TypeMemberDeclaration> TypeDeclaration.Members => Members;

        public static ClassDeclaration Create(IClassDeclarationSyntax syntax, bool isAbstract, UnresolvedSupertypeName? baseTypeName, IEnumerable<ClassMemberDeclaration> members, Symbol containingSymbol, UserTypeSymbol symbol, DeclarationScope newScope, IEnumerable<GenericParameter> genericParameters, IEnumerable<UnresolvedSupertypeName> supertypeNames, CodeFile file, DeclarationLexicalScope containingScope)
            => new ClassDeclarationNode(syntax, isAbstract, baseTypeName, members.ToFixedList(), containingSymbol, symbol, newScope, genericParameters.ToFixedList(), supertypeNames.ToFixedList(), file, containingScope);
    }

    public partial interface StructDeclaration : TypeDeclaration
    {
        new IStructDeclarationSyntax Syntax { get; }
        ITypeDeclarationSyntax TypeDeclaration.Syntax => Syntax;
        new IFixedList<StructMemberDeclaration> Members { get; }
        IFixedList<TypeMemberDeclaration> TypeDeclaration.Members => Members;

        public static StructDeclaration Create(IStructDeclarationSyntax syntax, IEnumerable<StructMemberDeclaration> members, Symbol containingSymbol, UserTypeSymbol symbol, DeclarationScope newScope, IEnumerable<GenericParameter> genericParameters, IEnumerable<UnresolvedSupertypeName> supertypeNames, CodeFile file, DeclarationLexicalScope containingScope)
            => new StructDeclarationNode(syntax, members.ToFixedList(), containingSymbol, symbol, newScope, genericParameters.ToFixedList(), supertypeNames.ToFixedList(), file, containingScope);
    }

    public partial interface TraitDeclaration : TypeDeclaration
    {
        new ITraitDeclarationSyntax Syntax { get; }
        ITypeDeclarationSyntax TypeDeclaration.Syntax => Syntax;
        new IFixedList<TraitMemberDeclaration> Members { get; }
        IFixedList<TypeMemberDeclaration> TypeDeclaration.Members => Members;

        public static TraitDeclaration Create(ITraitDeclarationSyntax syntax, IEnumerable<TraitMemberDeclaration> members, Symbol containingSymbol, UserTypeSymbol symbol, DeclarationScope newScope, IEnumerable<GenericParameter> genericParameters, IEnumerable<UnresolvedSupertypeName> supertypeNames, CodeFile file, DeclarationLexicalScope containingScope)
            => new TraitDeclarationNode(syntax, members.ToFixedList(), containingSymbol, symbol, newScope, genericParameters.ToFixedList(), supertypeNames.ToFixedList(), file, containingScope);
    }

    public partial interface GenericParameter : Code
    {
        new IGenericParameterSyntax Syntax { get; }
        IConcreteSyntax Code.Syntax => Syntax;
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
        IConcreteSyntax Code.Syntax => Syntax;
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
        IConcreteSyntax Code.Syntax => Syntax;
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

        public static UnresolvedIdentifierTypeName Create(IIdentifierTypeNameSyntax syntax, IdentifierName name, DeclarationLexicalScope containingScope)
            => new UnresolvedIdentifierTypeNameNode(syntax, name, containingScope);
    }

    public partial interface UnresolvedSpecialTypeName : UnresolvedSimpleTypeName
    {
        new ISpecialTypeNameSyntax Syntax { get; }
        ISimpleTypeNameSyntax UnresolvedSimpleTypeName.Syntax => Syntax;
        new SpecialTypeName Name { get; }
        TypeName UnresolvedTypeName.Name => Name;

        public static UnresolvedSpecialTypeName Create(ISpecialTypeNameSyntax syntax, SpecialTypeName name, DeclarationLexicalScope containingScope)
            => new UnresolvedSpecialTypeNameNode(syntax, name, containingScope);
    }

    public partial interface UnresolvedGenericTypeName : UnresolvedStandardTypeName
    {
        new IGenericTypeNameSyntax Syntax { get; }
        IStandardTypeNameSyntax UnresolvedStandardTypeName.Syntax => Syntax;
        new GenericName Name { get; }
        StandardName UnresolvedStandardTypeName.Name => Name;
        IFixedList<UnresolvedType> TypeArguments { get; }

        public static UnresolvedGenericTypeName Create(IGenericTypeNameSyntax syntax, GenericName name, IEnumerable<UnresolvedType> typeArguments, DeclarationLexicalScope containingScope)
            => new UnresolvedGenericTypeNameNode(syntax, name, typeArguments.ToFixedList(), containingScope);
    }

    public partial interface UnresolvedQualifiedTypeName : UnresolvedTypeName
    {
        new IQualifiedTypeNameSyntax Syntax { get; }
        ITypeNameSyntax UnresolvedTypeName.Syntax => Syntax;
        UnresolvedTypeName Context { get; }
        UnresolvedStandardTypeName QualifiedName { get; }

        public static UnresolvedQualifiedTypeName Create(IQualifiedTypeNameSyntax syntax, UnresolvedTypeName context, UnresolvedStandardTypeName qualifiedName, DeclarationLexicalScope containingScope, TypeName name)
            => new UnresolvedQualifiedTypeNameNode(syntax, context, qualifiedName, containingScope, name);
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

public sealed partial class WithTypeDeclarationPromises
{
    public partial interface UnresolvedSupertypeName : WithTypeDeclarationSymbols.UnresolvedSupertypeName
    {
        IFixedList<WithTypeDeclarationSymbols.UnresolvedType> WithTypeDeclarationSymbols.UnresolvedSupertypeName.TypeArguments => TypeArguments;
    }

    public partial interface UnresolvedTypeName : WithTypeDeclarationSymbols.UnresolvedTypeName
    {
    }

    public partial interface FunctionDeclaration : WithTypeDeclarationSymbols.FunctionDeclaration
    {
    }

    public partial interface PackageReference : WithTypeDeclarationSymbols.PackageReference
    {
    }

    public partial interface GenericParameter : WithTypeDeclarationSymbols.GenericParameter
    {
        WithTypeDeclarationSymbols.CapabilityConstraint WithTypeDeclarationSymbols.GenericParameter.Constraint => Constraint;
    }

    public partial interface CapabilityConstraint : WithTypeDeclarationSymbols.CapabilityConstraint
    {
    }

    public partial interface CapabilitySet : WithTypeDeclarationSymbols.CapabilitySet
    {
    }

    public partial interface Capability : WithTypeDeclarationSymbols.Capability
    {
    }

    public partial interface UnresolvedType : WithTypeDeclarationSymbols.UnresolvedType
    {
    }

    public partial interface UnresolvedStandardTypeName : WithTypeDeclarationSymbols.UnresolvedStandardTypeName
    {
    }

    public partial interface UnresolvedSimpleTypeName : WithTypeDeclarationSymbols.UnresolvedSimpleTypeName
    {
    }

    public partial interface UnresolvedIdentifierTypeName : WithTypeDeclarationSymbols.UnresolvedIdentifierTypeName
    {
    }

    public partial interface UnresolvedSpecialTypeName : WithTypeDeclarationSymbols.UnresolvedSpecialTypeName
    {
    }

    public partial interface UnresolvedGenericTypeName : WithTypeDeclarationSymbols.UnresolvedGenericTypeName
    {
        IFixedList<WithTypeDeclarationSymbols.UnresolvedType> WithTypeDeclarationSymbols.UnresolvedGenericTypeName.TypeArguments => TypeArguments;
    }

    public partial interface UnresolvedQualifiedTypeName : WithTypeDeclarationSymbols.UnresolvedQualifiedTypeName
    {
        WithTypeDeclarationSymbols.UnresolvedTypeName WithTypeDeclarationSymbols.UnresolvedQualifiedTypeName.Context => Context;
        WithTypeDeclarationSymbols.UnresolvedStandardTypeName WithTypeDeclarationSymbols.UnresolvedQualifiedTypeName.QualifiedName => QualifiedName;
    }

    public partial interface UnresolvedOptionalType : WithTypeDeclarationSymbols.UnresolvedOptionalType
    {
        WithTypeDeclarationSymbols.UnresolvedType WithTypeDeclarationSymbols.UnresolvedOptionalType.Referent => Referent;
    }

    public partial interface UnresolvedCapabilityType : WithTypeDeclarationSymbols.UnresolvedCapabilityType
    {
        WithTypeDeclarationSymbols.Capability WithTypeDeclarationSymbols.UnresolvedCapabilityType.Capability => Capability;
        WithTypeDeclarationSymbols.UnresolvedType WithTypeDeclarationSymbols.UnresolvedCapabilityType.Referent => Referent;
    }

    public partial interface UnresolvedFunctionType : WithTypeDeclarationSymbols.UnresolvedFunctionType
    {
        IFixedList<WithTypeDeclarationSymbols.UnresolvedParameterType> WithTypeDeclarationSymbols.UnresolvedFunctionType.Parameters => Parameters;
        WithTypeDeclarationSymbols.UnresolvedType WithTypeDeclarationSymbols.UnresolvedFunctionType.Return => Return;
    }

    public partial interface UnresolvedParameterType : WithTypeDeclarationSymbols.UnresolvedParameterType
    {
        WithTypeDeclarationSymbols.UnresolvedType WithTypeDeclarationSymbols.UnresolvedParameterType.Referent => Referent;
    }

    public partial interface UnresolvedViewpointType : WithTypeDeclarationSymbols.UnresolvedViewpointType
    {
        WithTypeDeclarationSymbols.UnresolvedType WithTypeDeclarationSymbols.UnresolvedViewpointType.Referent => Referent;
    }

    public partial interface UnresolvedCapabilityViewpointType : WithTypeDeclarationSymbols.UnresolvedCapabilityViewpointType
    {
        WithTypeDeclarationSymbols.Capability WithTypeDeclarationSymbols.UnresolvedCapabilityViewpointType.Capability => Capability;
    }

    public partial interface UnresolvedSelfViewpointType : WithTypeDeclarationSymbols.UnresolvedSelfViewpointType
    {
    }

}

