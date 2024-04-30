using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.IST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Contexts;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using From = Azoth.Tools.Bootstrap.Compiler.IST.Concrete;
using To = Azoth.Tools.Bootstrap.Compiler.IST.WithNamespaceSymbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Namespaces;

[GeneratedCode("AzothCompilerCodeGen", null)]
internal sealed partial class NamespaceSymbolBuilder : ITransformPass<From.Package, SymbolBuilderContext, To.Package, Void>
{
    public static To.Package Run(From.Package from, SymbolBuilderContext context)
    {
        var pass = new NamespaceSymbolBuilder(context);
        pass.StartRun();
        var to = pass.TransformPackage(from);
        pass.EndRun(to);
        return to;
    }

    static (To.Package, Void) ITransformPass<From.Package, SymbolBuilderContext, To.Package, Void>.Run(From.Package from, SymbolBuilderContext context)
        => (Run(from, context), default);


    partial void StartRun();

    partial void EndRun(To.Package to);

    private partial To.Package TransformPackage(From.Package from);

    private partial To.CompilationUnit TransformCompilationUnit(From.CompilationUnit from, PackageSymbol containingNamespace, ISymbolTreeBuilder treeBuilder);

    private partial To.FunctionDeclaration TransformFunctionDeclaration(From.FunctionDeclaration from, NamespaceSymbol containingNamespace);

    private partial To.NamespaceDeclaration TransformNamespaceDeclaration(From.NamespaceDeclaration from, NamespaceSymbol containingNamespace, ISymbolTreeBuilder treeBuilder);

    private partial To.TypeDeclaration TransformTypeDeclaration(From.TypeDeclaration from, NamespaceSymbol? containingNamespace);

    private IFixedSet<To.CompilationUnit> TransformCompilationUnits(IEnumerable<From.CompilationUnit> from, PackageSymbol containingNamespace, ISymbolTreeBuilder treeBuilder)
        => from.Select(f => TransformCompilationUnit(f, containingNamespace, treeBuilder)).ToFixedSet();

    private To.NamespaceMemberDeclaration TransformNamespaceMemberDeclaration(From.NamespaceMemberDeclaration from, NamespaceSymbol containingNamespace, ISymbolTreeBuilder treeBuilder)
        => from switch
        {
            From.NamespaceDeclaration f => TransformNamespaceDeclaration(f, containingNamespace, treeBuilder),
            From.TypeDeclaration f => TransformTypeDeclaration(f, containingNamespace),
            From.FunctionDeclaration f => TransformFunctionDeclaration(f, containingNamespace),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private IFixedList<To.NamespaceMemberDeclaration> TransformNamespaceMemberDeclarations(IEnumerable<From.NamespaceMemberDeclaration> from, NamespaceSymbol containingNamespace, ISymbolTreeBuilder treeBuilder)
        => from.Select(f => TransformNamespaceMemberDeclaration(f, containingNamespace, treeBuilder)).ToFixedList();

    private To.ClassMemberDeclaration TransformClassMemberDeclaration(From.ClassMemberDeclaration from, NamespaceSymbol? containingNamespace)
        => from switch
        {
            From.TypeDeclaration f => TransformTypeDeclaration(f, containingNamespace),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.TypeMemberDeclaration TransformTypeMemberDeclaration(From.TypeMemberDeclaration from, NamespaceSymbol? containingNamespace)
        => from switch
        {
            From.ClassMemberDeclaration f => TransformClassMemberDeclaration(f, containingNamespace),
            From.TraitMemberDeclaration f => TransformTraitMemberDeclaration(f, containingNamespace),
            From.StructMemberDeclaration f => TransformStructMemberDeclaration(f, containingNamespace),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private IFixedList<To.TypeMemberDeclaration> TransformTypeMemberDeclarations(IEnumerable<From.TypeMemberDeclaration> from, NamespaceSymbol? containingNamespace)
        => from.Select(f => TransformTypeMemberDeclaration(f, containingNamespace)).ToFixedList();

    private IFixedList<To.ClassMemberDeclaration> TransformClassMemberDeclarations(IEnumerable<From.ClassMemberDeclaration> from, NamespaceSymbol? containingNamespace)
        => from.Select(f => TransformClassMemberDeclaration(f, containingNamespace)).ToFixedList();

    private To.ClassDeclaration TransformClassDeclaration(From.ClassDeclaration from, NamespaceSymbol? containingNamespace, NamespaceSymbol? childContainingNamespace)
        => CreateClassDeclaration(from, containingNamespace, childContainingNamespace);

    private To.StructMemberDeclaration TransformStructMemberDeclaration(From.StructMemberDeclaration from, NamespaceSymbol? containingNamespace)
        => from switch
        {
            From.TypeDeclaration f => TransformTypeDeclaration(f, containingNamespace),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private IFixedList<To.StructMemberDeclaration> TransformStructMemberDeclarations(IEnumerable<From.StructMemberDeclaration> from, NamespaceSymbol? containingNamespace)
        => from.Select(f => TransformStructMemberDeclaration(f, containingNamespace)).ToFixedList();

    private To.StructDeclaration TransformStructDeclaration(From.StructDeclaration from, NamespaceSymbol? containingNamespace, NamespaceSymbol? childContainingNamespace)
        => CreateStructDeclaration(from, containingNamespace, childContainingNamespace);

    private To.TraitMemberDeclaration TransformTraitMemberDeclaration(From.TraitMemberDeclaration from, NamespaceSymbol? containingNamespace)
        => from switch
        {
            From.TypeDeclaration f => TransformTypeDeclaration(f, containingNamespace),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private IFixedList<To.TraitMemberDeclaration> TransformTraitMemberDeclarations(IEnumerable<From.TraitMemberDeclaration> from, NamespaceSymbol? containingNamespace)
        => from.Select(f => TransformTraitMemberDeclaration(f, containingNamespace)).ToFixedList();

    private To.TraitDeclaration TransformTraitDeclaration(From.TraitDeclaration from, NamespaceSymbol? containingNamespace, NamespaceSymbol? childContainingNamespace)
        => CreateTraitDeclaration(from, containingNamespace, childContainingNamespace);

    #region Create() methods
    private To.NamespaceDeclaration CreateNamespaceDeclaration(From.NamespaceDeclaration from, NamespaceSymbol containingNamespace, NamespaceSymbol symbol, IEnumerable<To.NamespaceMemberDeclaration> declarations)
        => To.NamespaceDeclaration.Create(containingNamespace, symbol, from.Syntax, from.IsGlobalQualified, from.DeclaredNames, from.UsingDirectives, declarations);

    private To.FunctionDeclaration CreateFunctionDeclaration(From.FunctionDeclaration from, NamespaceSymbol containingNamespace)
        => To.FunctionDeclaration.Create(containingNamespace, from.Syntax);

    private To.Package CreatePackage(From.Package from, IEnumerable<To.CompilationUnit> compilationUnits, IEnumerable<To.CompilationUnit> testingCompilationUnits)
        => To.Package.Create(from.Syntax, from.Symbol, from.References, compilationUnits, testingCompilationUnits);

    private To.PackageReference CreatePackageReference(From.PackageReference from)
        => To.PackageReference.Create(from.Syntax, from.AliasOrName, from.Package, from.IsTrusted);

    private To.CompilationUnit CreateCompilationUnit(From.CompilationUnit from, IEnumerable<To.NamespaceMemberDeclaration> declarations)
        => To.CompilationUnit.Create(from.Syntax, from.File, from.ImplicitNamespaceName, from.UsingDirectives, declarations);

    private To.UsingDirective CreateUsingDirective(From.UsingDirective from)
        => To.UsingDirective.Create(from.Syntax, from.Name);

    private To.ClassDeclaration CreateClassDeclaration(From.ClassDeclaration from, IEnumerable<To.ClassMemberDeclaration> members, NamespaceSymbol? containingNamespace)
        => To.ClassDeclaration.Create(from.Syntax, from.IsAbstract, from.BaseTypeName, members, from.GenericParameters, from.SupertypeNames, containingNamespace);

    private To.StructDeclaration CreateStructDeclaration(From.StructDeclaration from, IEnumerable<To.StructMemberDeclaration> members, NamespaceSymbol? containingNamespace)
        => To.StructDeclaration.Create(from.Syntax, members, from.GenericParameters, from.SupertypeNames, containingNamespace);

    private To.TraitDeclaration CreateTraitDeclaration(From.TraitDeclaration from, IEnumerable<To.TraitMemberDeclaration> members, NamespaceSymbol? containingNamespace)
        => To.TraitDeclaration.Create(from.Syntax, members, from.GenericParameters, from.SupertypeNames, containingNamespace);

    private To.GenericParameter CreateGenericParameter(From.GenericParameter from)
        => To.GenericParameter.Create(from.Syntax, from.Constraint, from.Name, from.Independence, from.Variance);

    private To.UnresolvedSupertypeName CreateUnresolvedSupertypeName(From.UnresolvedSupertypeName from)
        => To.UnresolvedSupertypeName.Create(from.Syntax, from.Name, from.TypeArguments);

    private To.CapabilitySet CreateCapabilitySet(From.CapabilitySet from)
        => To.CapabilitySet.Create(from.Syntax, from.Constraint);

    private To.Capability CreateCapability(From.Capability from)
        => To.Capability.Create(from.Syntax, from.Capability, from.Constraint);

    private To.UnresolvedIdentifierTypeName CreateUnresolvedIdentifierTypeName(From.UnresolvedIdentifierTypeName from)
        => To.UnresolvedIdentifierTypeName.Create(from.Syntax, from.Name);

    private To.UnresolvedSpecialTypeName CreateUnresolvedSpecialTypeName(From.UnresolvedSpecialTypeName from)
        => To.UnresolvedSpecialTypeName.Create(from.Syntax, from.Name);

    private To.UnresolvedGenericTypeName CreateUnresolvedGenericTypeName(From.UnresolvedGenericTypeName from)
        => To.UnresolvedGenericTypeName.Create(from.Syntax, from.Name, from.TypeArguments);

    private To.UnresolvedQualifiedTypeName CreateUnresolvedQualifiedTypeName(From.UnresolvedQualifiedTypeName from)
        => To.UnresolvedQualifiedTypeName.Create(from.Syntax, from.Context, from.QualifiedName, from.Name);

    private To.UnresolvedOptionalType CreateUnresolvedOptionalType(From.UnresolvedOptionalType from)
        => To.UnresolvedOptionalType.Create(from.Syntax, from.Referent);

    private To.UnresolvedCapabilityType CreateUnresolvedCapabilityType(From.UnresolvedCapabilityType from)
        => To.UnresolvedCapabilityType.Create(from.Syntax, from.Capability, from.Referent);

    private To.UnresolvedFunctionType CreateUnresolvedFunctionType(From.UnresolvedFunctionType from)
        => To.UnresolvedFunctionType.Create(from.Syntax, from.Parameters, from.Return);

    private To.UnresolvedParameterType CreateUnresolvedParameterType(From.UnresolvedParameterType from)
        => To.UnresolvedParameterType.Create(from.Syntax, from.IsLent, from.Referent);

    private To.UnresolvedCapabilityViewpointType CreateUnresolvedCapabilityViewpointType(From.UnresolvedCapabilityViewpointType from)
        => To.UnresolvedCapabilityViewpointType.Create(from.Syntax, from.Capability, from.Referent);

    private To.UnresolvedSelfViewpointType CreateUnresolvedSelfViewpointType(From.UnresolvedSelfViewpointType from)
        => To.UnresolvedSelfViewpointType.Create(from.Syntax, from.Referent);

    #endregion

    #region CreateX() methods
/*  private To.Declaration CreateDeclaration(From.Declaration from, NamespaceSymbol containingNamespace, NamespaceSymbol symbol)
        => from switch
        {
            From.NamespaceMemberDeclaration f => CreateNamespaceMemberDeclaration(f, containingNamespace, symbol),
            From.TypeMemberDeclaration f => CreateTypeMemberDeclaration(f, containingNamespace),
            _ => throw ExhaustiveMatch.Failed(from),
        }; */

/*  private To.NamespaceDeclaration CreateNamespaceDeclaration(From.NamespaceDeclaration from, NamespaceSymbol containingNamespace, NamespaceSymbol symbol)
        => To.NamespaceDeclaration.Create(containingNamespace, symbol, from.Syntax, from.IsGlobalQualified, from.DeclaredNames, from.UsingDirectives, TransformNamespaceMemberDeclarations(from.Declarations, childContainingNamespace, childTreeBuilder)); */

/*  private To.Package CreatePackage(From.Package from)
        => To.Package.Create(from.Syntax, from.Symbol, from.References, TransformCompilationUnits(from.CompilationUnits, childContainingNamespace, childTreeBuilder), TransformCompilationUnits(from.TestingCompilationUnits, childContainingNamespace, childTreeBuilder)); */

/*  private To.CompilationUnit CreateCompilationUnit(From.CompilationUnit from)
        => To.CompilationUnit.Create(from.Syntax, from.File, from.ImplicitNamespaceName, from.UsingDirectives, TransformNamespaceMemberDeclarations(from.Declarations, childContainingNamespace, childTreeBuilder)); */

/*  private To.Code CreateCode(From.Code from, NamespaceSymbol containingNamespace, NamespaceSymbol symbol)
        => from switch
        {
            From.Declaration f => CreateDeclaration(f, containingNamespace, symbol),
            From.CompilationUnit f => CreateCompilationUnit(f),
            From.UsingDirective f => CreateUsingDirective(f),
            From.GenericParameter f => CreateGenericParameter(f),
            From.UnresolvedSupertypeName f => CreateUnresolvedSupertypeName(f),
            From.CapabilityConstraint f => CreateCapabilityConstraint(f),
            From.UnresolvedType f => CreateUnresolvedType(f),
            _ => throw ExhaustiveMatch.Failed(from),
        }; */

/*  private To.NamespaceMemberDeclaration CreateNamespaceMemberDeclaration(From.NamespaceMemberDeclaration from, NamespaceSymbol containingNamespace, NamespaceSymbol symbol)
        => from switch
        {
            From.NamespaceDeclaration f => CreateNamespaceDeclaration(f, containingNamespace, symbol),
            From.FunctionDeclaration f => CreateFunctionDeclaration(f, containingNamespace),
            From.TypeDeclaration f => CreateTypeDeclaration(f, containingNamespace),
            _ => throw ExhaustiveMatch.Failed(from),
        }; */

/*  private To.TypeDeclaration CreateTypeDeclaration(From.TypeDeclaration from, NamespaceSymbol? containingNamespace)
        => from switch
        {
            From.ClassDeclaration f => CreateClassDeclaration(f, containingNamespace),
            From.StructDeclaration f => CreateStructDeclaration(f, containingNamespace),
            From.TraitDeclaration f => CreateTraitDeclaration(f, containingNamespace),
            _ => throw ExhaustiveMatch.Failed(from),
        }; */

/*  private To.ClassDeclaration CreateClassDeclaration(From.ClassDeclaration from, NamespaceSymbol? containingNamespace)
        => To.ClassDeclaration.Create(from.Syntax, from.IsAbstract, from.BaseTypeName, TransformClassMemberDeclarations(from.Members, childContainingNamespace), from.GenericParameters, from.SupertypeNames, containingNamespace); */

/*  private To.StructDeclaration CreateStructDeclaration(From.StructDeclaration from, NamespaceSymbol? containingNamespace)
        => To.StructDeclaration.Create(from.Syntax, TransformStructMemberDeclarations(from.Members, childContainingNamespace), from.GenericParameters, from.SupertypeNames, containingNamespace); */

/*  private To.TraitDeclaration CreateTraitDeclaration(From.TraitDeclaration from, NamespaceSymbol? containingNamespace)
        => To.TraitDeclaration.Create(from.Syntax, TransformTraitMemberDeclarations(from.Members, childContainingNamespace), from.GenericParameters, from.SupertypeNames, containingNamespace); */

/*  private To.TypeMemberDeclaration CreateTypeMemberDeclaration(From.TypeMemberDeclaration from, NamespaceSymbol? containingNamespace)
        => from switch
        {
            From.ClassMemberDeclaration f => CreateClassMemberDeclaration(f, containingNamespace),
            From.TraitMemberDeclaration f => CreateTraitMemberDeclaration(f, containingNamespace),
            From.StructMemberDeclaration f => CreateStructMemberDeclaration(f, containingNamespace),
            _ => throw ExhaustiveMatch.Failed(from),
        }; */

/*  private To.ClassMemberDeclaration CreateClassMemberDeclaration(From.ClassMemberDeclaration from, NamespaceSymbol? containingNamespace)
        => from switch
        {
            From.TypeDeclaration f => CreateTypeDeclaration(f, containingNamespace),
            _ => throw ExhaustiveMatch.Failed(from),
        }; */

/*  private To.TraitMemberDeclaration CreateTraitMemberDeclaration(From.TraitMemberDeclaration from, NamespaceSymbol? containingNamespace)
        => from switch
        {
            From.TypeDeclaration f => CreateTypeDeclaration(f, containingNamespace),
            _ => throw ExhaustiveMatch.Failed(from),
        }; */

/*  private To.StructMemberDeclaration CreateStructMemberDeclaration(From.StructMemberDeclaration from, NamespaceSymbol? containingNamespace)
        => from switch
        {
            From.TypeDeclaration f => CreateTypeDeclaration(f, containingNamespace),
            _ => throw ExhaustiveMatch.Failed(from),
        }; */

/*  private To.CapabilityConstraint CreateCapabilityConstraint(From.CapabilityConstraint from)
        => from switch
        {
            From.CapabilitySet f => CreateCapabilitySet(f),
            From.Capability f => CreateCapability(f),
            _ => throw ExhaustiveMatch.Failed(from),
        }; */

/*  private To.UnresolvedType CreateUnresolvedType(From.UnresolvedType from)
        => from switch
        {
            From.UnresolvedTypeName f => CreateUnresolvedTypeName(f),
            From.UnresolvedOptionalType f => CreateUnresolvedOptionalType(f),
            From.UnresolvedCapabilityType f => CreateUnresolvedCapabilityType(f),
            From.UnresolvedFunctionType f => CreateUnresolvedFunctionType(f),
            From.UnresolvedViewpointType f => CreateUnresolvedViewpointType(f),
            _ => throw ExhaustiveMatch.Failed(from),
        }; */

/*  private To.UnresolvedTypeName CreateUnresolvedTypeName(From.UnresolvedTypeName from)
        => from switch
        {
            From.UnresolvedStandardTypeName f => CreateUnresolvedStandardTypeName(f),
            From.UnresolvedSimpleTypeName f => CreateUnresolvedSimpleTypeName(f),
            From.UnresolvedQualifiedTypeName f => CreateUnresolvedQualifiedTypeName(f),
            _ => throw ExhaustiveMatch.Failed(from),
        }; */

/*  private To.UnresolvedStandardTypeName CreateUnresolvedStandardTypeName(From.UnresolvedStandardTypeName from)
        => from switch
        {
            From.UnresolvedIdentifierTypeName f => CreateUnresolvedIdentifierTypeName(f),
            From.UnresolvedGenericTypeName f => CreateUnresolvedGenericTypeName(f),
            _ => throw ExhaustiveMatch.Failed(from),
        }; */

/*  private To.UnresolvedSimpleTypeName CreateUnresolvedSimpleTypeName(From.UnresolvedSimpleTypeName from)
        => from switch
        {
            From.UnresolvedIdentifierTypeName f => CreateUnresolvedIdentifierTypeName(f),
            From.UnresolvedSpecialTypeName f => CreateUnresolvedSpecialTypeName(f),
            _ => throw ExhaustiveMatch.Failed(from),
        }; */

/*  private To.UnresolvedViewpointType CreateUnresolvedViewpointType(From.UnresolvedViewpointType from)
        => from switch
        {
            From.UnresolvedCapabilityViewpointType f => CreateUnresolvedCapabilityViewpointType(f),
            From.UnresolvedSelfViewpointType f => CreateUnresolvedSelfViewpointType(f),
            _ => throw ExhaustiveMatch.Failed(from),
        }; */

    private To.Package CreatePackage(From.Package from, PackageSymbol childContainingNamespace, ISymbolTreeBuilder childTreeBuilder)
        => To.Package.Create(from.Syntax, from.Symbol, from.References, TransformCompilationUnits(from.CompilationUnits, childContainingNamespace, childTreeBuilder), TransformCompilationUnits(from.TestingCompilationUnits, childContainingNamespace, childTreeBuilder));

    private To.CompilationUnit CreateCompilationUnit(From.CompilationUnit from, NamespaceSymbol childContainingNamespace, ISymbolTreeBuilder childTreeBuilder)
        => To.CompilationUnit.Create(from.Syntax, from.File, from.ImplicitNamespaceName, from.UsingDirectives, TransformNamespaceMemberDeclarations(from.Declarations, childContainingNamespace, childTreeBuilder));

    private To.NamespaceDeclaration CreateNamespaceDeclaration(From.NamespaceDeclaration from, NamespaceSymbol containingNamespace, NamespaceSymbol symbol, NamespaceSymbol childContainingNamespace, ISymbolTreeBuilder childTreeBuilder)
        => To.NamespaceDeclaration.Create(containingNamespace, symbol, from.Syntax, from.IsGlobalQualified, from.DeclaredNames, from.UsingDirectives, TransformNamespaceMemberDeclarations(from.Declarations, childContainingNamespace, childTreeBuilder));

    private To.TypeDeclaration CreateTypeDeclaration(From.TypeDeclaration from, NamespaceSymbol? containingNamespace, NamespaceSymbol? childContainingNamespace)
        => from switch
        {
            From.ClassDeclaration f => CreateClassDeclaration(f, containingNamespace, childContainingNamespace),
            From.StructDeclaration f => CreateStructDeclaration(f, containingNamespace, childContainingNamespace),
            From.TraitDeclaration f => CreateTraitDeclaration(f, containingNamespace, childContainingNamespace),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.ClassDeclaration CreateClassDeclaration(From.ClassDeclaration from, NamespaceSymbol? containingNamespace, NamespaceSymbol? childContainingNamespace)
        => To.ClassDeclaration.Create(from.Syntax, from.IsAbstract, from.BaseTypeName, TransformClassMemberDeclarations(from.Members, childContainingNamespace), from.GenericParameters, from.SupertypeNames, containingNamespace);

    private To.StructDeclaration CreateStructDeclaration(From.StructDeclaration from, NamespaceSymbol? containingNamespace, NamespaceSymbol? childContainingNamespace)
        => To.StructDeclaration.Create(from.Syntax, TransformStructMemberDeclarations(from.Members, childContainingNamespace), from.GenericParameters, from.SupertypeNames, containingNamespace);

    private To.TraitDeclaration CreateTraitDeclaration(From.TraitDeclaration from, NamespaceSymbol? containingNamespace, NamespaceSymbol? childContainingNamespace)
        => To.TraitDeclaration.Create(from.Syntax, TransformTraitMemberDeclarations(from.Members, childContainingNamespace), from.GenericParameters, from.SupertypeNames, containingNamespace);

    #endregion
}
