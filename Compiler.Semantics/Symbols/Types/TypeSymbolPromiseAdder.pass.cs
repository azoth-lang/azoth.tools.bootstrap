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
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using From = Azoth.Tools.Bootstrap.Compiler.IST.WithoutCompilationUnits;
using To = Azoth.Tools.Bootstrap.Compiler.IST.WithTypeDeclarationPromises;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Types;

[GeneratedCode("AzothCompilerCodeGen", null)]
internal sealed partial class TypeSymbolPromiseAdder : ITransformPass<From.Package, Void, To.Package, Void>
{
    public static To.Package Run(From.Package from)
    {
        var pass = new TypeSymbolPromiseAdder();
        pass.StartRun();
        var to = pass.TransformPackage(from);
        pass.EndRun(to);
        return to;
    }

    static (To.Package, Void) ITransformPass<From.Package, Void, To.Package, Void>.Run(From.Package from, Void context)
        => (Run(from), default);


    partial void StartRun();

    partial void EndRun(To.Package to);

    private partial To.Package TransformPackage(From.Package from);

    private partial To.TypeDeclaration TransformTypeDeclaration(From.TypeDeclaration from, IPromise<Symbol>? containingSymbol);

    private To.FunctionDeclaration TransformFunctionDeclaration(From.FunctionDeclaration from)
        => from;

    private To.NamespaceMemberDeclaration TransformNamespaceMemberDeclaration(From.NamespaceMemberDeclaration from, IPromise<Symbol>? containingSymbol)
        => from switch
        {
            From.TypeDeclaration f => TransformTypeDeclaration(f, containingSymbol),
            From.FunctionDeclaration f => TransformFunctionDeclaration(f),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private IFixedSet<To.NamespaceMemberDeclaration> TransformNamespaceMemberDeclarations(IEnumerable<From.NamespaceMemberDeclaration> from, IPromise<Symbol>? containingSymbol)
        => from.Select(f => TransformNamespaceMemberDeclaration(f, containingSymbol)).ToFixedSet();

    private To.ClassMemberDeclaration TransformClassMemberDeclaration(From.ClassMemberDeclaration from, IPromise<Symbol>? containingSymbol)
        => from switch
        {
            From.TypeDeclaration f => TransformTypeDeclaration(f, containingSymbol),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private IFixedList<To.ClassMemberDeclaration> TransformClassMemberDeclarations(IEnumerable<From.ClassMemberDeclaration> from, IPromise<Symbol>? containingSymbol)
        => from.Select(f => TransformClassMemberDeclaration(f, containingSymbol)).ToFixedList();

    private To.ClassDeclaration TransformClassDeclaration(From.ClassDeclaration from, AcyclicPromise<UserTypeSymbol> symbol, IPromise<Symbol> containingSymbol, IPromise<Symbol>? childContainingSymbol)
        => CreateClassDeclaration(from, symbol, containingSymbol, childContainingSymbol);

    private To.StructMemberDeclaration TransformStructMemberDeclaration(From.StructMemberDeclaration from, IPromise<Symbol>? containingSymbol)
        => from switch
        {
            From.TypeDeclaration f => TransformTypeDeclaration(f, containingSymbol),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private IFixedList<To.StructMemberDeclaration> TransformStructMemberDeclarations(IEnumerable<From.StructMemberDeclaration> from, IPromise<Symbol>? containingSymbol)
        => from.Select(f => TransformStructMemberDeclaration(f, containingSymbol)).ToFixedList();

    private To.StructDeclaration TransformStructDeclaration(From.StructDeclaration from, AcyclicPromise<UserTypeSymbol> symbol, IPromise<Symbol> containingSymbol, IPromise<Symbol>? childContainingSymbol)
        => CreateStructDeclaration(from, symbol, containingSymbol, childContainingSymbol);

    private To.TraitMemberDeclaration TransformTraitMemberDeclaration(From.TraitMemberDeclaration from, IPromise<Symbol>? containingSymbol)
        => from switch
        {
            From.TypeDeclaration f => TransformTypeDeclaration(f, containingSymbol),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private IFixedList<To.TraitMemberDeclaration> TransformTraitMemberDeclarations(IEnumerable<From.TraitMemberDeclaration> from, IPromise<Symbol>? containingSymbol)
        => from.Select(f => TransformTraitMemberDeclaration(f, containingSymbol)).ToFixedList();

    private To.TraitDeclaration TransformTraitDeclaration(From.TraitDeclaration from, AcyclicPromise<UserTypeSymbol> symbol, IPromise<Symbol> containingSymbol, IPromise<Symbol>? childContainingSymbol)
        => CreateTraitDeclaration(from, symbol, containingSymbol, childContainingSymbol);

    #region Create() methods
    private To.Package CreatePackage(From.Package from, IEnumerable<To.NamespaceMemberDeclaration> declarations, IEnumerable<To.NamespaceMemberDeclaration> testingDeclarations)
        => To.Package.Create(declarations, testingDeclarations, from.LexicalScope, from.Syntax, from.Symbol, from.References);

    private To.UnresolvedSupertypeName CreateUnresolvedSupertypeName(From.UnresolvedSupertypeName from)
        => To.UnresolvedSupertypeName.Create(from.ContainingScope, from.Syntax, from.Name, from.TypeArguments);

    private To.FunctionDeclaration CreateFunctionDeclaration(From.FunctionDeclaration from)
        => To.FunctionDeclaration.Create(from.ContainingNamespace, from.Syntax, from.File, from.ContainingScope);

    private To.PackageReference CreatePackageReference(From.PackageReference from)
        => To.PackageReference.Create(from.Syntax, from.AliasOrName, from.Package, from.IsTrusted);

    private To.ClassDeclaration CreateClassDeclaration(From.ClassDeclaration from, IEnumerable<To.ClassMemberDeclaration> members, AcyclicPromise<UserTypeSymbol> symbol, IPromise<Symbol> containingSymbol)
        => To.ClassDeclaration.Create(from.Syntax, from.IsAbstract, from.BaseTypeName, members, symbol, containingSymbol, from.NewScope, from.GenericParameters, from.SupertypeNames, from.File, from.ContainingScope, from.ContainingNamespace);

    private To.StructDeclaration CreateStructDeclaration(From.StructDeclaration from, IEnumerable<To.StructMemberDeclaration> members, AcyclicPromise<UserTypeSymbol> symbol, IPromise<Symbol> containingSymbol)
        => To.StructDeclaration.Create(from.Syntax, members, symbol, containingSymbol, from.NewScope, from.GenericParameters, from.SupertypeNames, from.File, from.ContainingScope, from.ContainingNamespace);

    private To.TraitDeclaration CreateTraitDeclaration(From.TraitDeclaration from, IEnumerable<To.TraitMemberDeclaration> members, AcyclicPromise<UserTypeSymbol> symbol, IPromise<Symbol> containingSymbol)
        => To.TraitDeclaration.Create(from.Syntax, members, symbol, containingSymbol, from.NewScope, from.GenericParameters, from.SupertypeNames, from.File, from.ContainingScope, from.ContainingNamespace);

    private To.GenericParameter CreateGenericParameter(From.GenericParameter from)
        => To.GenericParameter.Create(from.Syntax, from.Constraint, from.Name, from.Independence, from.Variance);

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
    private To.Package CreatePackage(From.Package from, IPromise<Symbol>? childContainingSymbol)
        => To.Package.Create(TransformNamespaceMemberDeclarations(from.Declarations, childContainingSymbol), TransformNamespaceMemberDeclarations(from.TestingDeclarations, childContainingSymbol), from.LexicalScope, from.Syntax, from.Symbol, from.References);

    private To.TypeDeclaration CreateTypeDeclaration(From.TypeDeclaration from, AcyclicPromise<UserTypeSymbol> symbol, IPromise<Symbol> containingSymbol, IPromise<Symbol>? childContainingSymbol)
        => from switch
        {
            From.ClassDeclaration f => CreateClassDeclaration(f, symbol, containingSymbol, childContainingSymbol),
            From.StructDeclaration f => CreateStructDeclaration(f, symbol, containingSymbol, childContainingSymbol),
            From.TraitDeclaration f => CreateTraitDeclaration(f, symbol, containingSymbol, childContainingSymbol),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.NamespaceMemberDeclaration CreateNamespaceMemberDeclaration(From.NamespaceMemberDeclaration from, AcyclicPromise<UserTypeSymbol> symbol, IPromise<Symbol> containingSymbol, IPromise<Symbol>? childContainingSymbol)
        => from switch
        {
            From.TypeDeclaration f => CreateTypeDeclaration(f, symbol, containingSymbol, childContainingSymbol),
            From.FunctionDeclaration f => CreateFunctionDeclaration(f),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.ClassMemberDeclaration CreateClassMemberDeclaration(From.ClassMemberDeclaration from, AcyclicPromise<UserTypeSymbol> symbol, IPromise<Symbol> containingSymbol, IPromise<Symbol>? childContainingSymbol)
        => from switch
        {
            From.TypeDeclaration f => CreateTypeDeclaration(f, symbol, containingSymbol, childContainingSymbol),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.ClassDeclaration CreateClassDeclaration(From.ClassDeclaration from, AcyclicPromise<UserTypeSymbol> symbol, IPromise<Symbol> containingSymbol, IPromise<Symbol>? childContainingSymbol)
        => To.ClassDeclaration.Create(from.Syntax, from.IsAbstract, from.BaseTypeName, TransformClassMemberDeclarations(from.Members, childContainingSymbol), symbol, containingSymbol, from.NewScope, from.GenericParameters, from.SupertypeNames, from.File, from.ContainingScope, from.ContainingNamespace);

    private To.StructMemberDeclaration CreateStructMemberDeclaration(From.StructMemberDeclaration from, AcyclicPromise<UserTypeSymbol> symbol, IPromise<Symbol> containingSymbol, IPromise<Symbol>? childContainingSymbol)
        => from switch
        {
            From.TypeDeclaration f => CreateTypeDeclaration(f, symbol, containingSymbol, childContainingSymbol),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.StructDeclaration CreateStructDeclaration(From.StructDeclaration from, AcyclicPromise<UserTypeSymbol> symbol, IPromise<Symbol> containingSymbol, IPromise<Symbol>? childContainingSymbol)
        => To.StructDeclaration.Create(from.Syntax, TransformStructMemberDeclarations(from.Members, childContainingSymbol), symbol, containingSymbol, from.NewScope, from.GenericParameters, from.SupertypeNames, from.File, from.ContainingScope, from.ContainingNamespace);

    private To.TraitMemberDeclaration CreateTraitMemberDeclaration(From.TraitMemberDeclaration from, AcyclicPromise<UserTypeSymbol> symbol, IPromise<Symbol> containingSymbol, IPromise<Symbol>? childContainingSymbol)
        => from switch
        {
            From.TypeDeclaration f => CreateTypeDeclaration(f, symbol, containingSymbol, childContainingSymbol),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.TraitDeclaration CreateTraitDeclaration(From.TraitDeclaration from, AcyclicPromise<UserTypeSymbol> symbol, IPromise<Symbol> containingSymbol, IPromise<Symbol>? childContainingSymbol)
        => To.TraitDeclaration.Create(from.Syntax, TransformTraitMemberDeclarations(from.Members, childContainingSymbol), symbol, containingSymbol, from.NewScope, from.GenericParameters, from.SupertypeNames, from.File, from.ContainingScope, from.ContainingNamespace);

    #endregion
}
