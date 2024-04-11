using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.IST;

// ReSharper disable MemberHidesStaticFromOuterClass

public sealed partial class Typed
{
    private sealed class PackageNode : Node, Package
    {
        public IPackageSyntax Syntax { get; }
        public PackageSymbol Symbol { get; }
        public IFixedSet<PackageReference> References { get; }
        public IFixedSet<CompilationUnit> CompilationUnits { get; }
        public IFixedSet<CompilationUnit> TestingCompilationUnits { get; }

        public PackageNode(IPackageSyntax syntax, PackageSymbol symbol, IFixedSet<PackageReference> references, IFixedSet<CompilationUnit> compilationUnits, IFixedSet<CompilationUnit> testingCompilationUnits)
        {
            Syntax = syntax;
            Symbol = symbol;
            References = references;
            CompilationUnits = compilationUnits;
            TestingCompilationUnits = testingCompilationUnits;
        }
    }

    private sealed class PackageReferenceNode : Node, PackageReference
    {
        public IPackageReferenceSyntax Syntax { get; }
        public IdentifierName AliasOrName { get; }
        public IPackageSymbols Package { get; }
        public bool IsTrusted { get; }

        public PackageReferenceNode(IPackageReferenceSyntax syntax, IdentifierName aliasOrName, IPackageSymbols package, bool isTrusted)
        {
            Syntax = syntax;
            AliasOrName = aliasOrName;
            Package = package;
            IsTrusted = isTrusted;
        }
    }

    private sealed class CompilationUnitNode : Node, CompilationUnit
    {
        public ICompilationUnitSyntax Syntax { get; }
        public CodeFile File { get; }
        public NamespaceName ImplicitNamespaceName { get; }
        public IFixedList<UsingDirective> UsingDirectives { get; }
        public IFixedList<NamespaceMemberDeclaration> Declarations { get; }

        public CompilationUnitNode(ICompilationUnitSyntax syntax, CodeFile file, NamespaceName implicitNamespaceName, IFixedList<UsingDirective> usingDirectives, IFixedList<NamespaceMemberDeclaration> declarations)
        {
            Syntax = syntax;
            File = file;
            ImplicitNamespaceName = implicitNamespaceName;
            UsingDirectives = usingDirectives;
            Declarations = declarations;
        }
    }

    private sealed class UsingDirectiveNode : Node, UsingDirective
    {
        public IUsingDirectiveSyntax Syntax { get; }
        public NamespaceName Name { get; }

        public UsingDirectiveNode(IUsingDirectiveSyntax syntax, NamespaceName name)
        {
            Syntax = syntax;
            Name = name;
        }
    }

    private sealed class NamespaceDeclarationNode : Node, NamespaceDeclaration
    {
        public INamespaceDeclarationSyntax Syntax { get; }
        public bool IsGlobalQualified { get; }
        public NamespaceName DeclaredNames { get; }
        public IFixedList<UsingDirective> UsingDirectives { get; }
        public IFixedList<NamespaceMemberDeclaration> Declarations { get; }
        public LexicalScope ContainingLexicalScope { get; }

        public NamespaceDeclarationNode(INamespaceDeclarationSyntax syntax, bool isGlobalQualified, NamespaceName declaredNames, IFixedList<UsingDirective> usingDirectives, IFixedList<NamespaceMemberDeclaration> declarations, LexicalScope containingLexicalScope)
        {
            Syntax = syntax;
            IsGlobalQualified = isGlobalQualified;
            DeclaredNames = declaredNames;
            UsingDirectives = usingDirectives;
            Declarations = declarations;
            ContainingLexicalScope = containingLexicalScope;
        }
    }

    private sealed class ClassDeclarationNode : Node, ClassDeclaration
    {
        public IClassDeclarationSyntax Syntax { get; }
        public bool IsAbstract { get; }
        public UnresolvedSupertypeName? BaseTypeName { get; }
        public IFixedList<ClassMemberDeclaration> Members { get; }
        public IFixedList<GenericParameter> GenericParameters { get; }
        public IFixedList<UnresolvedSupertypeName> SupertypeNames { get; }
        public LexicalScope ContainingLexicalScope { get; }

        public ClassDeclarationNode(IClassDeclarationSyntax syntax, bool isAbstract, UnresolvedSupertypeName? baseTypeName, IFixedList<ClassMemberDeclaration> members, IFixedList<GenericParameter> genericParameters, IFixedList<UnresolvedSupertypeName> supertypeNames, LexicalScope containingLexicalScope)
        {
            Syntax = syntax;
            IsAbstract = isAbstract;
            BaseTypeName = baseTypeName;
            Members = members;
            GenericParameters = genericParameters;
            SupertypeNames = supertypeNames;
            ContainingLexicalScope = containingLexicalScope;
        }
    }

    private sealed class StructDeclarationNode : Node, StructDeclaration
    {
        public IStructDeclarationSyntax Syntax { get; }
        public IFixedList<StructMemberDeclaration> Members { get; }
        public IFixedList<GenericParameter> GenericParameters { get; }
        public IFixedList<UnresolvedSupertypeName> SupertypeNames { get; }
        public LexicalScope ContainingLexicalScope { get; }

        public StructDeclarationNode(IStructDeclarationSyntax syntax, IFixedList<StructMemberDeclaration> members, IFixedList<GenericParameter> genericParameters, IFixedList<UnresolvedSupertypeName> supertypeNames, LexicalScope containingLexicalScope)
        {
            Syntax = syntax;
            Members = members;
            GenericParameters = genericParameters;
            SupertypeNames = supertypeNames;
            ContainingLexicalScope = containingLexicalScope;
        }
    }

    private sealed class TraitDeclarationNode : Node, TraitDeclaration
    {
        public ITraitDeclarationSyntax Syntax { get; }
        public IFixedList<TraitMemberDeclaration> Members { get; }
        public IFixedList<GenericParameter> GenericParameters { get; }
        public IFixedList<UnresolvedSupertypeName> SupertypeNames { get; }
        public LexicalScope ContainingLexicalScope { get; }

        public TraitDeclarationNode(ITraitDeclarationSyntax syntax, IFixedList<TraitMemberDeclaration> members, IFixedList<GenericParameter> genericParameters, IFixedList<UnresolvedSupertypeName> supertypeNames, LexicalScope containingLexicalScope)
        {
            Syntax = syntax;
            Members = members;
            GenericParameters = genericParameters;
            SupertypeNames = supertypeNames;
            ContainingLexicalScope = containingLexicalScope;
        }
    }

    private sealed class GenericParameterNode : Node, GenericParameter
    {
        public IGenericParameterSyntax Syntax { get; }
        public CapabilityConstraint Constraint { get; }
        public IdentifierName Name { get; }
        public ParameterIndependence Independence { get; }
        public ParameterVariance Variance { get; }

        public GenericParameterNode(IGenericParameterSyntax syntax, CapabilityConstraint constraint, IdentifierName name, ParameterIndependence independence, ParameterVariance variance)
        {
            Syntax = syntax;
            Constraint = constraint;
            Name = name;
            Independence = independence;
            Variance = variance;
        }
    }

    private sealed class UnresolvedSupertypeNameNode : Node, UnresolvedSupertypeName
    {
        public ISupertypeNameSyntax Syntax { get; }
        public TypeName Name { get; }
        public IFixedList<UnresolvedType> TypeArguments { get; }

        public UnresolvedSupertypeNameNode(ISupertypeNameSyntax syntax, TypeName name, IFixedList<UnresolvedType> typeArguments)
        {
            Syntax = syntax;
            Name = name;
            TypeArguments = typeArguments;
        }
    }

    private sealed class FunctionDeclarationNode : Node, FunctionDeclaration
    {
        public IFunctionDeclarationSyntax Syntax { get; }
        public LexicalScope ContainingLexicalScope { get; }

        public FunctionDeclarationNode(IFunctionDeclarationSyntax syntax, LexicalScope containingLexicalScope)
        {
            Syntax = syntax;
            ContainingLexicalScope = containingLexicalScope;
        }
    }

    private sealed class CapabilitySetNode : Node, CapabilitySet
    {
        public ICapabilitySetSyntax Syntax { get; }
        public Types.Capabilities.CapabilitySet Constraint { get; }

        public CapabilitySetNode(ICapabilitySetSyntax syntax, Types.Capabilities.CapabilitySet constraint)
        {
            Syntax = syntax;
            Constraint = constraint;
        }
    }

    private sealed class CapabilityNode : Node, Capability
    {
        public ICapabilitySyntax Syntax { get; }
        public Types.Capabilities.Capability Capability { get; }
        public Types.Capabilities.Capability Constraint { get; }

        public CapabilityNode(ICapabilitySyntax syntax, Types.Capabilities.Capability capability, Types.Capabilities.Capability constraint)
        {
            Syntax = syntax;
            Capability = capability;
            Constraint = constraint;
        }
    }

    private sealed class UnresolvedIdentifierTypeNameNode : Node, UnresolvedIdentifierTypeName
    {
        public IIdentifierTypeNameSyntax Syntax { get; }
        public IdentifierName Name { get; }

        public UnresolvedIdentifierTypeNameNode(IIdentifierTypeNameSyntax syntax, IdentifierName name)
        {
            Syntax = syntax;
            Name = name;
        }
    }

    private sealed class UnresolvedSpecialTypeNameNode : Node, UnresolvedSpecialTypeName
    {
        public ISpecialTypeNameSyntax Syntax { get; }
        public SpecialTypeName Name { get; }

        public UnresolvedSpecialTypeNameNode(ISpecialTypeNameSyntax syntax, SpecialTypeName name)
        {
            Syntax = syntax;
            Name = name;
        }
    }

    private sealed class UnresolvedGenericTypeNameNode : Node, UnresolvedGenericTypeName
    {
        public IGenericTypeNameSyntax Syntax { get; }
        public GenericName Name { get; }
        public IFixedList<UnresolvedType> TypeArguments { get; }

        public UnresolvedGenericTypeNameNode(IGenericTypeNameSyntax syntax, GenericName name, IFixedList<UnresolvedType> typeArguments)
        {
            Syntax = syntax;
            Name = name;
            TypeArguments = typeArguments;
        }
    }

    private sealed class UnresolvedQualifiedTypeNameNode : Node, UnresolvedQualifiedTypeName
    {
        public IQualifiedTypeNameSyntax Syntax { get; }
        public UnresolvedTypeName Context { get; }
        public UnresolvedStandardTypeName QualifiedName { get; }
        public TypeName Name { get; }

        public UnresolvedQualifiedTypeNameNode(IQualifiedTypeNameSyntax syntax, UnresolvedTypeName context, UnresolvedStandardTypeName qualifiedName, TypeName name)
        {
            Syntax = syntax;
            Context = context;
            QualifiedName = qualifiedName;
            Name = name;
        }
    }

    private sealed class UnresolvedOptionalTypeNode : Node, UnresolvedOptionalType
    {
        public IOptionalTypeSyntax Syntax { get; }
        public UnresolvedType Referent { get; }

        public UnresolvedOptionalTypeNode(IOptionalTypeSyntax syntax, UnresolvedType referent)
        {
            Syntax = syntax;
            Referent = referent;
        }
    }

    private sealed class UnresolvedCapabilityTypeNode : Node, UnresolvedCapabilityType
    {
        public ICapabilityTypeSyntax Syntax { get; }
        public Capability Capability { get; }
        public UnresolvedType Referent { get; }

        public UnresolvedCapabilityTypeNode(ICapabilityTypeSyntax syntax, Capability capability, UnresolvedType referent)
        {
            Syntax = syntax;
            Capability = capability;
            Referent = referent;
        }
    }

    private sealed class UnresolvedFunctionTypeNode : Node, UnresolvedFunctionType
    {
        public IFunctionTypeSyntax Syntax { get; }
        public IFixedList<UnresolvedParameterType> Parameters { get; }
        public UnresolvedType Return { get; }

        public UnresolvedFunctionTypeNode(IFunctionTypeSyntax syntax, IFixedList<UnresolvedParameterType> parameters, UnresolvedType @return)
        {
            Syntax = syntax;
            Parameters = parameters;
            Return = @return;
        }
    }

    private sealed class UnresolvedParameterTypeNode : Node, UnresolvedParameterType
    {
        public IParameterTypeSyntax Syntax { get; }
        public bool IsLent { get; }
        public UnresolvedType Referent { get; }

        public UnresolvedParameterTypeNode(IParameterTypeSyntax syntax, bool isLent, UnresolvedType referent)
        {
            Syntax = syntax;
            IsLent = isLent;
            Referent = referent;
        }
    }

    private sealed class UnresolvedCapabilityViewpointTypeNode : Node, UnresolvedCapabilityViewpointType
    {
        public ICapabilityViewpointTypeSyntax Syntax { get; }
        public Capability Capability { get; }
        public UnresolvedType Referent { get; }

        public UnresolvedCapabilityViewpointTypeNode(ICapabilityViewpointTypeSyntax syntax, Capability capability, UnresolvedType referent)
        {
            Syntax = syntax;
            Capability = capability;
            Referent = referent;
        }
    }

    private sealed class UnresolvedSelfViewpointTypeNode : Node, UnresolvedSelfViewpointType
    {
        public ISelfViewpointTypeSyntax Syntax { get; }
        public UnresolvedType Referent { get; }

        public UnresolvedSelfViewpointTypeNode(ISelfViewpointTypeSyntax syntax, UnresolvedType referent)
        {
            Syntax = syntax;
            Referent = referent;
        }
    }

}
