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

public sealed partial class WithoutCompilationUnits
{
    private sealed class PackageNode : Node, Package
    {
        public IFixedSet<NamespaceMemberDeclaration> Declarations { get; }
        public IFixedSet<NamespaceMemberDeclaration> TestingDeclarations { get; }
        public PackageReferenceScope LexicalScope { get; }
        public IPackageSyntax Syntax { get; }
        public PackageSymbol Symbol { get; }
        public IFixedSet<PackageReference> References { get; }

        public PackageNode(IFixedSet<NamespaceMemberDeclaration> declarations, IFixedSet<NamespaceMemberDeclaration> testingDeclarations, PackageReferenceScope lexicalScope, IPackageSyntax syntax, PackageSymbol symbol, IFixedSet<PackageReference> references)
        {
            Declarations = declarations;
            TestingDeclarations = testingDeclarations;
            LexicalScope = lexicalScope;
            Syntax = syntax;
            Symbol = symbol;
            References = references;
        }
    }

    private sealed class UnresolvedSupertypeNameNode : Node, UnresolvedSupertypeName
    {
        public DeclarationLexicalScope ContainingScope { get; }
        public ISupertypeNameSyntax Syntax { get; }
        public TypeName Name { get; }
        public IFixedList<UnresolvedType> TypeArguments { get; }

        public UnresolvedSupertypeNameNode(DeclarationLexicalScope containingScope, ISupertypeNameSyntax syntax, TypeName name, IFixedList<UnresolvedType> typeArguments)
        {
            ContainingScope = containingScope;
            Syntax = syntax;
            Name = name;
            TypeArguments = typeArguments;
        }
    }

    private sealed class FunctionDeclarationNode : Node, FunctionDeclaration
    {
        public NamespaceSymbol ContainingSymbol { get; }
        public IFunctionDeclarationSyntax Syntax { get; }
        public CodeFile File { get; }
        public DeclarationLexicalScope ContainingScope { get; }

        public FunctionDeclarationNode(NamespaceSymbol containingSymbol, IFunctionDeclarationSyntax syntax, CodeFile file, DeclarationLexicalScope containingScope)
        {
            ContainingSymbol = containingSymbol;
            Syntax = syntax;
            File = file;
            ContainingScope = containingScope;
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

    private sealed class ClassDeclarationNode : Node, ClassDeclaration
    {
        public IClassDeclarationSyntax Syntax { get; }
        public bool IsAbstract { get; }
        public UnresolvedSupertypeName? BaseTypeName { get; }
        public IFixedList<ClassMemberDeclaration> Members { get; }
        public DeclarationScope NewScope { get; }
        public IFixedList<GenericParameter> GenericParameters { get; }
        public IFixedList<UnresolvedSupertypeName> SupertypeNames { get; }
        public CodeFile File { get; }
        public DeclarationLexicalScope ContainingScope { get; }
        public NamespaceSymbol? ContainingSymbol { get; }

        public ClassDeclarationNode(IClassDeclarationSyntax syntax, bool isAbstract, UnresolvedSupertypeName? baseTypeName, IFixedList<ClassMemberDeclaration> members, DeclarationScope newScope, IFixedList<GenericParameter> genericParameters, IFixedList<UnresolvedSupertypeName> supertypeNames, CodeFile file, DeclarationLexicalScope containingScope, NamespaceSymbol? containingSymbol)
        {
            Syntax = syntax;
            IsAbstract = isAbstract;
            BaseTypeName = baseTypeName;
            Members = members;
            NewScope = newScope;
            GenericParameters = genericParameters;
            SupertypeNames = supertypeNames;
            File = file;
            ContainingScope = containingScope;
            ContainingSymbol = containingSymbol;
        }
    }

    private sealed class StructDeclarationNode : Node, StructDeclaration
    {
        public IStructDeclarationSyntax Syntax { get; }
        public IFixedList<StructMemberDeclaration> Members { get; }
        public DeclarationScope NewScope { get; }
        public IFixedList<GenericParameter> GenericParameters { get; }
        public IFixedList<UnresolvedSupertypeName> SupertypeNames { get; }
        public CodeFile File { get; }
        public DeclarationLexicalScope ContainingScope { get; }
        public NamespaceSymbol? ContainingSymbol { get; }

        public StructDeclarationNode(IStructDeclarationSyntax syntax, IFixedList<StructMemberDeclaration> members, DeclarationScope newScope, IFixedList<GenericParameter> genericParameters, IFixedList<UnresolvedSupertypeName> supertypeNames, CodeFile file, DeclarationLexicalScope containingScope, NamespaceSymbol? containingSymbol)
        {
            Syntax = syntax;
            Members = members;
            NewScope = newScope;
            GenericParameters = genericParameters;
            SupertypeNames = supertypeNames;
            File = file;
            ContainingScope = containingScope;
            ContainingSymbol = containingSymbol;
        }
    }

    private sealed class TraitDeclarationNode : Node, TraitDeclaration
    {
        public ITraitDeclarationSyntax Syntax { get; }
        public IFixedList<TraitMemberDeclaration> Members { get; }
        public DeclarationScope NewScope { get; }
        public IFixedList<GenericParameter> GenericParameters { get; }
        public IFixedList<UnresolvedSupertypeName> SupertypeNames { get; }
        public CodeFile File { get; }
        public DeclarationLexicalScope ContainingScope { get; }
        public NamespaceSymbol? ContainingSymbol { get; }

        public TraitDeclarationNode(ITraitDeclarationSyntax syntax, IFixedList<TraitMemberDeclaration> members, DeclarationScope newScope, IFixedList<GenericParameter> genericParameters, IFixedList<UnresolvedSupertypeName> supertypeNames, CodeFile file, DeclarationLexicalScope containingScope, NamespaceSymbol? containingSymbol)
        {
            Syntax = syntax;
            Members = members;
            NewScope = newScope;
            GenericParameters = genericParameters;
            SupertypeNames = supertypeNames;
            File = file;
            ContainingScope = containingScope;
            ContainingSymbol = containingSymbol;
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
        public DeclarationLexicalScope ContainingScope { get; }

        public UnresolvedIdentifierTypeNameNode(IIdentifierTypeNameSyntax syntax, IdentifierName name, DeclarationLexicalScope containingScope)
        {
            Syntax = syntax;
            Name = name;
            ContainingScope = containingScope;
        }
    }

    private sealed class UnresolvedSpecialTypeNameNode : Node, UnresolvedSpecialTypeName
    {
        public ISpecialTypeNameSyntax Syntax { get; }
        public SpecialTypeName Name { get; }
        public DeclarationLexicalScope ContainingScope { get; }

        public UnresolvedSpecialTypeNameNode(ISpecialTypeNameSyntax syntax, SpecialTypeName name, DeclarationLexicalScope containingScope)
        {
            Syntax = syntax;
            Name = name;
            ContainingScope = containingScope;
        }
    }

    private sealed class UnresolvedGenericTypeNameNode : Node, UnresolvedGenericTypeName
    {
        public IGenericTypeNameSyntax Syntax { get; }
        public GenericName Name { get; }
        public IFixedList<UnresolvedType> TypeArguments { get; }
        public DeclarationLexicalScope ContainingScope { get; }

        public UnresolvedGenericTypeNameNode(IGenericTypeNameSyntax syntax, GenericName name, IFixedList<UnresolvedType> typeArguments, DeclarationLexicalScope containingScope)
        {
            Syntax = syntax;
            Name = name;
            TypeArguments = typeArguments;
            ContainingScope = containingScope;
        }
    }

    private sealed class UnresolvedQualifiedTypeNameNode : Node, UnresolvedQualifiedTypeName
    {
        public IQualifiedTypeNameSyntax Syntax { get; }
        public UnresolvedTypeName Context { get; }
        public UnresolvedStandardTypeName QualifiedName { get; }
        public DeclarationLexicalScope ContainingScope { get; }
        public TypeName Name { get; }

        public UnresolvedQualifiedTypeNameNode(IQualifiedTypeNameSyntax syntax, UnresolvedTypeName context, UnresolvedStandardTypeName qualifiedName, DeclarationLexicalScope containingScope, TypeName name)
        {
            Syntax = syntax;
            Context = context;
            QualifiedName = qualifiedName;
            ContainingScope = containingScope;
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
