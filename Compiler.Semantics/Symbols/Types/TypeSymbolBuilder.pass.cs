using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.IST;
using Azoth.Tools.Bootstrap.Compiler.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Contexts;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using From = Azoth.Tools.Bootstrap.Compiler.IST.WithTypeDeclarationPromises;
using To = Azoth.Tools.Bootstrap.Compiler.IST.WithTypeDeclarationSymbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Types;

[GeneratedCode("AzothCompilerCodeGen", null)]
internal sealed partial class TypeSymbolBuilder : ITransformPass<From.Package, SymbolBuilderContext, To.Package, SymbolBuilderContext>
{
    public static (To.Package, SymbolBuilderContext) Run(From.Package from, SymbolBuilderContext context)
    {
        var pass = new TypeSymbolBuilder(context);
        pass.StartRun();
        var to = pass.TransformPackage(from);
        var toContext = pass.EndRun(to);
        return (to, toContext);
    }


    partial void StartRun();

    private partial SymbolBuilderContext EndRun(To.Package to);

    private partial To.Package TransformPackage(From.Package from);

    private partial To.TypeDeclaration TransformTypeDeclaration(From.TypeDeclaration from, TypeLookup typeDeclarations);

    private To.ClassDeclaration TransformClassDeclaration(From.ClassDeclaration from, Symbol containingSymbol, UserTypeSymbol symbol, TypeLookup childTypeDeclarations)
        => CreateClassDeclaration(from, containingSymbol, symbol, childTypeDeclarations);

    private To.StructDeclaration TransformStructDeclaration(From.StructDeclaration from, Symbol containingSymbol, UserTypeSymbol symbol, TypeLookup childTypeDeclarations)
        => CreateStructDeclaration(from, containingSymbol, symbol, childTypeDeclarations);

    private To.TraitDeclaration TransformTraitDeclaration(From.TraitDeclaration from, Symbol containingSymbol, UserTypeSymbol symbol, TypeLookup childTypeDeclarations)
        => CreateTraitDeclaration(from, containingSymbol, symbol, childTypeDeclarations);

    private To.NamespaceMemberDeclaration TransformNamespaceMemberDeclaration(From.NamespaceMemberDeclaration from, TypeLookup typeDeclarations)
        => from switch
        {
            From.TypeDeclaration f => TransformTypeDeclaration(f, typeDeclarations),
            From.FunctionDeclaration f => f,
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.ClassMemberDeclaration TransformClassMemberDeclaration(From.ClassMemberDeclaration from, TypeLookup typeDeclarations)
        => from switch
        {
            From.TypeDeclaration f => TransformTypeDeclaration(f, typeDeclarations),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.TraitMemberDeclaration TransformTraitMemberDeclaration(From.TraitMemberDeclaration from, TypeLookup typeDeclarations)
        => from switch
        {
            From.TypeDeclaration f => TransformTypeDeclaration(f, typeDeclarations),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.StructMemberDeclaration TransformStructMemberDeclaration(From.StructMemberDeclaration from, TypeLookup typeDeclarations)
        => from switch
        {
            From.TypeDeclaration f => TransformTypeDeclaration(f, typeDeclarations),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.Declaration TransformDeclaration(From.Declaration from, TypeLookup typeDeclarations)
        => from switch
        {
            From.NamespaceMemberDeclaration f => TransformNamespaceMemberDeclaration(f, typeDeclarations),
            From.TypeMemberDeclaration f => TransformTypeMemberDeclaration(f, typeDeclarations),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private IFixedSet<To.NamespaceMemberDeclaration> TransformNamespaceMemberDeclarations(IEnumerable<From.NamespaceMemberDeclaration> from, TypeLookup typeDeclarations)
        => from.Select(f => TransformNamespaceMemberDeclaration(f, typeDeclarations)).ToFixedSet();

    private To.TypeMemberDeclaration TransformTypeMemberDeclaration(From.TypeMemberDeclaration from, TypeLookup typeDeclarations)
        => from switch
        {
            From.ClassMemberDeclaration f => TransformClassMemberDeclaration(f, typeDeclarations),
            From.TraitMemberDeclaration f => TransformTraitMemberDeclaration(f, typeDeclarations),
            From.StructMemberDeclaration f => TransformStructMemberDeclaration(f, typeDeclarations),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private IFixedList<To.ClassMemberDeclaration> TransformClassMemberDeclarations(IEnumerable<From.ClassMemberDeclaration> from, TypeLookup typeDeclarations)
        => from.Select(f => TransformClassMemberDeclaration(f, typeDeclarations)).ToFixedList();

    private IFixedList<To.TraitMemberDeclaration> TransformTraitMemberDeclarations(IEnumerable<From.TraitMemberDeclaration> from, TypeLookup typeDeclarations)
        => from.Select(f => TransformTraitMemberDeclaration(f, typeDeclarations)).ToFixedList();

    private IFixedList<To.StructMemberDeclaration> TransformStructMemberDeclarations(IEnumerable<From.StructMemberDeclaration> from, TypeLookup typeDeclarations)
        => from.Select(f => TransformStructMemberDeclaration(f, typeDeclarations)).ToFixedList();

    private To.Code TransformCode(From.Code from, TypeLookup typeDeclarations)
        => from switch
        {
            From.Declaration f => TransformDeclaration(f, typeDeclarations),
            From.GenericParameter f => TransformGenericParameter(f),
            From.UnresolvedSupertypeName f => f,
            From.CapabilityConstraint f => f,
            From.UnresolvedType f => f,
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private IFixedList<To.TypeMemberDeclaration> TransformTypeMemberDeclarations(IEnumerable<From.TypeMemberDeclaration> from, TypeLookup typeDeclarations)
        => from.Select(f => TransformTypeMemberDeclaration(f, typeDeclarations)).ToFixedList();

    private To.GenericParameter TransformGenericParameter(From.GenericParameter from)
        => CreateGenericParameter(from);

    private IFixedList<To.GenericParameter> TransformGenericParameters(IEnumerable<From.GenericParameter> from)
        => from.Select(f => TransformGenericParameter(f)).ToFixedList();

    #region Create() methods
    private To.GenericParameter CreateGenericParameter(From.GenericParameter from)
        => To.GenericParameter.Create(from.Symbol, from.Syntax, from.Constraint, from.Name, from.Independence, from.Variance);

    private To.Package CreatePackage(From.Package from, IEnumerable<To.NamespaceMemberDeclaration> declarations, IEnumerable<To.NamespaceMemberDeclaration> testingDeclarations)
        => To.Package.Create(declarations, testingDeclarations, from.LexicalScope, from.Syntax, from.Symbol, from.References);

    private To.UnresolvedSupertypeName CreateUnresolvedSupertypeName(From.UnresolvedSupertypeName from)
        => To.UnresolvedSupertypeName.Create(from.ContainingScope, from.Syntax, from.Name, from.TypeArguments);

    private To.FunctionDeclaration CreateFunctionDeclaration(From.FunctionDeclaration from)
        => To.FunctionDeclaration.Create(from.ContainingNamespace, from.Syntax, from.File, from.ContainingScope);

    private To.PackageReference CreatePackageReference(From.PackageReference from)
        => To.PackageReference.Create(from.Syntax, from.AliasOrName, from.Package, from.IsTrusted);

    private To.ClassDeclaration CreateClassDeclaration(From.ClassDeclaration from, IEnumerable<To.ClassMemberDeclaration> members, Symbol containingSymbol, UserTypeSymbol symbol)
        => To.ClassDeclaration.Create(from.Syntax, from.IsAbstract, from.BaseTypeName, members, containingSymbol, symbol, from.NewScope, from.GenericParameters, from.SupertypeNames, from.File, from.ContainingScope);

    private To.StructDeclaration CreateStructDeclaration(From.StructDeclaration from, IEnumerable<To.StructMemberDeclaration> members, Symbol containingSymbol, UserTypeSymbol symbol)
        => To.StructDeclaration.Create(from.Syntax, members, containingSymbol, symbol, from.NewScope, from.GenericParameters, from.SupertypeNames, from.File, from.ContainingScope);

    private To.TraitDeclaration CreateTraitDeclaration(From.TraitDeclaration from, IEnumerable<To.TraitMemberDeclaration> members, Symbol containingSymbol, UserTypeSymbol symbol)
        => To.TraitDeclaration.Create(from.Syntax, members, containingSymbol, symbol, from.NewScope, from.GenericParameters, from.SupertypeNames, from.File, from.ContainingScope);

    private To.CapabilitySet CreateCapabilitySet(From.CapabilitySet from)
        => To.CapabilitySet.Create(from.Syntax, from.Constraint);

    private To.Capability CreateCapability(From.Capability from)
        => To.Capability.Create(from.Syntax, from.Capability, from.Constraint);

    private To.UnresolvedIdentifierTypeName CreateUnresolvedIdentifierTypeName(From.UnresolvedIdentifierTypeName from)
        => To.UnresolvedIdentifierTypeName.Create(from.Syntax, from.Name, from.ContainingScope);

    private To.UnresolvedSpecialTypeName CreateUnresolvedSpecialTypeName(From.UnresolvedSpecialTypeName from)
        => To.UnresolvedSpecialTypeName.Create(from.Syntax, from.Name, from.ContainingScope);

    private To.UnresolvedGenericTypeName CreateUnresolvedGenericTypeName(From.UnresolvedGenericTypeName from)
        => To.UnresolvedGenericTypeName.Create(from.Syntax, from.Name, from.TypeArguments, from.ContainingScope);

    private To.UnresolvedQualifiedTypeName CreateUnresolvedQualifiedTypeName(From.UnresolvedQualifiedTypeName from)
        => To.UnresolvedQualifiedTypeName.Create(from.Syntax, from.Context, from.QualifiedName, from.ContainingScope, from.Name);

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
    private To.TypeDeclaration CreateTypeDeclaration(From.TypeDeclaration from, Symbol containingSymbol, UserTypeSymbol symbol, TypeLookup childTypeDeclarations)
        => from switch
        {
            From.ClassDeclaration f => CreateClassDeclaration(f, containingSymbol, symbol, childTypeDeclarations),
            From.StructDeclaration f => CreateStructDeclaration(f, containingSymbol, symbol, childTypeDeclarations),
            From.TraitDeclaration f => CreateTraitDeclaration(f, containingSymbol, symbol, childTypeDeclarations),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.Declaration CreateDeclaration(From.Declaration from, Symbol containingSymbol, UserTypeSymbol symbol, TypeLookup childTypeDeclarations)
        => from switch
        {
            From.NamespaceMemberDeclaration f => CreateNamespaceMemberDeclaration(f, containingSymbol, symbol, childTypeDeclarations),
            From.TypeMemberDeclaration f => CreateTypeMemberDeclaration(f, containingSymbol, symbol, childTypeDeclarations),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.Package CreatePackage(From.Package from, TypeLookup childTypeDeclarations)
        => To.Package.Create(TransformNamespaceMemberDeclarations(from.Declarations, childTypeDeclarations), TransformNamespaceMemberDeclarations(from.TestingDeclarations, childTypeDeclarations), from.LexicalScope, from.Syntax, from.Symbol, from.References);

    private To.NamespaceMemberDeclaration CreateNamespaceMemberDeclaration(From.NamespaceMemberDeclaration from, Symbol containingSymbol, UserTypeSymbol symbol, TypeLookup childTypeDeclarations)
        => from switch
        {
            From.TypeDeclaration f => CreateTypeDeclaration(f, containingSymbol, symbol, childTypeDeclarations),
            From.FunctionDeclaration f => CreateFunctionDeclaration(f),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.UnresolvedTypeName CreateUnresolvedTypeName(From.UnresolvedTypeName from)
        => from switch
        {
            From.UnresolvedStandardTypeName f => CreateUnresolvedStandardTypeName(f),
            From.UnresolvedSimpleTypeName f => CreateUnresolvedSimpleTypeName(f),
            From.UnresolvedQualifiedTypeName f => CreateUnresolvedQualifiedTypeName(f),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.Code CreateCode(From.Code from, Symbol containingSymbol, UserTypeSymbol symbol, TypeLookup childTypeDeclarations)
        => from switch
        {
            From.Declaration f => CreateDeclaration(f, containingSymbol, symbol, childTypeDeclarations),
            From.GenericParameter f => CreateGenericParameter(f),
            From.UnresolvedSupertypeName f => CreateUnresolvedSupertypeName(f),
            From.CapabilityConstraint f => CreateCapabilityConstraint(f),
            From.UnresolvedType f => CreateUnresolvedType(f),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.ClassDeclaration CreateClassDeclaration(From.ClassDeclaration from, Symbol containingSymbol, UserTypeSymbol symbol, TypeLookup childTypeDeclarations)
        => To.ClassDeclaration.Create(from.Syntax, from.IsAbstract, from.BaseTypeName, TransformClassMemberDeclarations(from.Members, childTypeDeclarations), containingSymbol, symbol, from.NewScope, from.GenericParameters, from.SupertypeNames, from.File, from.ContainingScope);

    private To.StructDeclaration CreateStructDeclaration(From.StructDeclaration from, Symbol containingSymbol, UserTypeSymbol symbol, TypeLookup childTypeDeclarations)
        => To.StructDeclaration.Create(from.Syntax, TransformStructMemberDeclarations(from.Members, childTypeDeclarations), containingSymbol, symbol, from.NewScope, from.GenericParameters, from.SupertypeNames, from.File, from.ContainingScope);

    private To.TraitDeclaration CreateTraitDeclaration(From.TraitDeclaration from, Symbol containingSymbol, UserTypeSymbol symbol, TypeLookup childTypeDeclarations)
        => To.TraitDeclaration.Create(from.Syntax, TransformTraitMemberDeclarations(from.Members, childTypeDeclarations), containingSymbol, symbol, from.NewScope, from.GenericParameters, from.SupertypeNames, from.File, from.ContainingScope);

    private To.TypeMemberDeclaration CreateTypeMemberDeclaration(From.TypeMemberDeclaration from, Symbol containingSymbol, UserTypeSymbol symbol, TypeLookup childTypeDeclarations)
        => from switch
        {
            From.ClassMemberDeclaration f => CreateClassMemberDeclaration(f, containingSymbol, symbol, childTypeDeclarations),
            From.TraitMemberDeclaration f => CreateTraitMemberDeclaration(f, containingSymbol, symbol, childTypeDeclarations),
            From.StructMemberDeclaration f => CreateStructMemberDeclaration(f, containingSymbol, symbol, childTypeDeclarations),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.ClassMemberDeclaration CreateClassMemberDeclaration(From.ClassMemberDeclaration from, Symbol containingSymbol, UserTypeSymbol symbol, TypeLookup childTypeDeclarations)
        => from switch
        {
            From.TypeDeclaration f => CreateTypeDeclaration(f, containingSymbol, symbol, childTypeDeclarations),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.TraitMemberDeclaration CreateTraitMemberDeclaration(From.TraitMemberDeclaration from, Symbol containingSymbol, UserTypeSymbol symbol, TypeLookup childTypeDeclarations)
        => from switch
        {
            From.TypeDeclaration f => CreateTypeDeclaration(f, containingSymbol, symbol, childTypeDeclarations),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.StructMemberDeclaration CreateStructMemberDeclaration(From.StructMemberDeclaration from, Symbol containingSymbol, UserTypeSymbol symbol, TypeLookup childTypeDeclarations)
        => from switch
        {
            From.TypeDeclaration f => CreateTypeDeclaration(f, containingSymbol, symbol, childTypeDeclarations),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.CapabilityConstraint CreateCapabilityConstraint(From.CapabilityConstraint from)
        => from switch
        {
            From.CapabilitySet f => CreateCapabilitySet(f),
            From.Capability f => CreateCapability(f),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.UnresolvedType CreateUnresolvedType(From.UnresolvedType from)
        => from switch
        {
            From.UnresolvedTypeName f => CreateUnresolvedTypeName(f),
            From.UnresolvedOptionalType f => CreateUnresolvedOptionalType(f),
            From.UnresolvedCapabilityType f => CreateUnresolvedCapabilityType(f),
            From.UnresolvedFunctionType f => CreateUnresolvedFunctionType(f),
            From.UnresolvedViewpointType f => CreateUnresolvedViewpointType(f),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.UnresolvedStandardTypeName CreateUnresolvedStandardTypeName(From.UnresolvedStandardTypeName from)
        => from switch
        {
            From.UnresolvedIdentifierTypeName f => CreateUnresolvedIdentifierTypeName(f),
            From.UnresolvedGenericTypeName f => CreateUnresolvedGenericTypeName(f),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.UnresolvedSimpleTypeName CreateUnresolvedSimpleTypeName(From.UnresolvedSimpleTypeName from)
        => from switch
        {
            From.UnresolvedIdentifierTypeName f => CreateUnresolvedIdentifierTypeName(f),
            From.UnresolvedSpecialTypeName f => CreateUnresolvedSpecialTypeName(f),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.UnresolvedViewpointType CreateUnresolvedViewpointType(From.UnresolvedViewpointType from)
        => from switch
        {
            From.UnresolvedCapabilityViewpointType f => CreateUnresolvedCapabilityViewpointType(f),
            From.UnresolvedSelfViewpointType f => CreateUnresolvedSelfViewpointType(f),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    #endregion
}
