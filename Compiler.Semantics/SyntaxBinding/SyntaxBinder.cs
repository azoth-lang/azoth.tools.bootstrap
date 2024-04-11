using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Contexts;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using ExhaustiveMatching;
using static Azoth.Tools.Bootstrap.Compiler.IST.Concrete;
using ASTPackage = Azoth.Tools.Bootstrap.Compiler.AST.Package;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.SyntaxBinding;

/// <summary>
/// Constructs the IST from the CST and binds it to the CST.
/// </summary>
internal sealed partial class SyntaxBinder
{
    private readonly Diagnostics diagnostics;

    private SyntaxBinder(DiagnosticsContext context)
    {
        diagnostics = context.Diagnostics;
    }

    private partial SymbolBuilderContext EndRun(Package package)
    {
        var packageSymbol = package.Symbol;
        // TODO remove downcast
        var packageSyntax = (PackageSyntax<ASTPackage>)package.Syntax;
        var symbolTree = packageSyntax.SymbolTree; // TODO new SymbolTreeBuilder(packageSymbol);
        var testingSymbolTree = packageSyntax.TestingSymbolTree; // TODO new SymbolTreeBuilder(symbolTree);
        return new SymbolBuilderContext(diagnostics, symbolTree, testingSymbolTree);
    }

    private partial Package Transform(IPackageSyntax from)
    {
        var symbol = new PackageSymbol(from.Name);
        return Package.Create(from, symbol, Transform(from.References),
            Transform(from.CompilationUnits), Transform(from.TestingCompilationUnits));
    }

    private partial PackageReference Transform(IPackageReferenceSyntax from)
        => PackageReference.Create(from, from.AliasOrName, from.Package, from.IsTrusted);

    private partial CompilationUnit Transform(ICompilationUnitSyntax from)
    {
        var usingDirectives = from.UsingDirectives.Select(Transform);
        var declarations = from.Declarations.Select(Transform);
        return CompilationUnit.Create(from, from.File, from.ImplicitNamespaceName, usingDirectives, declarations);
    }

    private UsingDirective Transform(IUsingDirectiveSyntax syntax)
        => UsingDirective.Create(syntax, syntax.Name);

    private NamespaceMemberDeclaration Transform(INonMemberDeclarationSyntax syntax)
        => syntax switch
        {
            INamespaceDeclarationSyntax syn => Transform(syn),
            ITypeDeclarationSyntax syn => Transform(syn),
            IFunctionDeclarationSyntax syn => Transform(syn),
            _ => throw ExhaustiveMatch.Failed(syntax),
        };

    private NamespaceDeclaration Transform(INamespaceDeclarationSyntax syntax)
    {
        var usingDirectives = syntax.UsingDirectives.Select(Transform);
        var declarations = syntax.Declarations.Select(Transform);
        return NamespaceDeclaration.Create(syntax, syntax.IsGlobalQualified, syntax.DeclaredNames, usingDirectives, declarations);
    }

    private FunctionDeclaration Transform(IFunctionDeclarationSyntax syntax)
        => FunctionDeclaration.Create(syntax);

    private TypeDeclaration Transform(ITypeDeclarationSyntax syntax)
        => syntax switch
        {
            IClassDeclarationSyntax syn => Transform(syn),
            IStructDeclarationSyntax syn => Transform(syn),
            ITraitDeclarationSyntax syn => Transform(syn),
            _ => throw ExhaustiveMatch.Failed(syntax),
        };

    private ClassDeclaration Transform(IClassDeclarationSyntax syntax)
    {
        var isAbstract = syntax.AbstractModifier is not null;
        var members = Enumerable.Empty<ClassMemberDeclaration>(); // TODO syntax.Members.Select(Build);
        var genericParameters = Transform(syntax.GenericParameters);
        var baseTypeName = Transform(syntax.BaseTypeName);
        var superTypeNames = Transform(syntax.SupertypeNames);
        return ClassDeclaration.Create(syntax, isAbstract, baseTypeName, members, genericParameters, superTypeNames);
    }

    private ClassMemberDeclaration Transform(IClassMemberDeclarationSyntax syntax)
        => throw new NotImplementedException();

    private StructDeclaration Transform(IStructDeclarationSyntax syntax)
    {
        var members = Enumerable.Empty<StructMemberDeclaration>(); // TODO syntax.Members.Select(Build);
        var genericParameters = Transform(syntax.GenericParameters);
        var superTypeNames = Transform(syntax.SupertypeNames);
        return StructDeclaration.Create(syntax, members, genericParameters, superTypeNames);
    }

    private StructMemberDeclaration Transform(IStructMemberDeclarationSyntax syntax)
        => throw new NotImplementedException();

    private TraitDeclaration Transform(ITraitDeclarationSyntax syntax)
    {
        var members = Enumerable.Empty<TraitMemberDeclaration>(); // TODO syntax.Members.Select(Build);
        var genericParameters = Transform(syntax.GenericParameters);
        var superTypeNames = Transform(syntax.SupertypeNames);
        return TraitDeclaration.Create(syntax, members, genericParameters, superTypeNames);
    }

    private TraitMemberDeclaration Transform(ITraitMemberDeclarationSyntax syntax)
        => throw new NotImplementedException();

    private partial GenericParameter Transform(IGenericParameterSyntax from)
        => GenericParameter.Create(from, Transform(from.Constraint), from.Name, from.Independence, from.Variance);

    private CapabilityConstraint Transform(ICapabilityConstraintSyntax from)
    {
        return from switch
        {
            ICapabilitySyntax f => Transform(f),
            ICapabilitySetSyntax f => Transform(f),
            _ => throw ExhaustiveMatch.Failed(from),
        };
    }

    private Capability Transform(ICapabilitySyntax from)
        // TODO fix to avoid needing to pass capability twice
        => Capability.Create(from, from.Capability, from.Capability);

    private CapabilitySet Transform(ICapabilitySetSyntax from)
        => CapabilitySet.Create(from, from.Constraint);

    private partial UnresolvedSupertypeName? Transform(ISupertypeNameSyntax? from)
    {
        if (from is null)
            return null;

        var typeArguments = Transform(from.TypeArguments);
        return UnresolvedSupertypeName.Create(from, from.Name, typeArguments);
    }

    private partial UnresolvedType Transform(ITypeSyntax from)
    {
        return from switch
        {
            ITypeNameSyntax f => Transform(f),
            IOptionalTypeSyntax f => Transform(f),
            ICapabilityTypeSyntax f => Transform(f),
            IFunctionTypeSyntax f => Transform(f),
            IViewpointTypeSyntax f => Transform(f),
            _ => throw ExhaustiveMatch.Failed(from),
        };
    }

    private UnresolvedTypeName Transform(ITypeNameSyntax from)
    {
        return from switch
        {
            IStandardTypeNameSyntax f => Transform(f),
            ISimpleTypeNameSyntax f => Transform(f),
            IQualifiedTypeNameSyntax f => Transform(f),
            _ => throw ExhaustiveMatch.Failed(from),
        };
    }

    private UnresolvedTypeName Transform(IStandardTypeNameSyntax from)
    {
        return from switch
        {
            IIdentifierTypeNameSyntax f => Transform(f),
            IGenericTypeNameSyntax f => Transform(f),
            _ => throw ExhaustiveMatch.Failed(from),
        };
    }

    private UnresolvedIdentifierTypeName Transform(IIdentifierTypeNameSyntax from)
        => UnresolvedIdentifierTypeName.Create(from, from.Name);

    private UnresolvedGenericTypeName Transform(IGenericTypeNameSyntax from)
        => UnresolvedGenericTypeName.Create(from, from.Name, Transform(from.TypeArguments));

    private UnresolvedSimpleTypeName Transform(ISimpleTypeNameSyntax from)
    {
        return from switch
        {
            IIdentifierTypeNameSyntax f => Transform(f),
            ISpecialTypeNameSyntax f => Transform(f),
            _ => throw ExhaustiveMatch.Failed(from),
        };
    }

    private UnresolvedSpecialTypeName Transform(ISpecialTypeNameSyntax from)
        => UnresolvedSpecialTypeName.Create(from, from.Name);

    private UnresolvedTypeName Transform(IQualifiedTypeNameSyntax from)
        => throw new NotImplementedException();

    private UnresolvedOptionalType Transform(IOptionalTypeSyntax from)
        => UnresolvedOptionalType.Create(from, Transform(from.Referent));

    private UnresolvedCapabilityType Transform(ICapabilityTypeSyntax from)
        => UnresolvedCapabilityType.Create(from, Transform(from.Capability), Transform(from.Referent));

    private UnresolvedFunctionType Transform(IFunctionTypeSyntax from)
        => throw new NotImplementedException();

    private UnresolvedViewpointType Transform(IViewpointTypeSyntax from)
        => throw new NotImplementedException();
}
