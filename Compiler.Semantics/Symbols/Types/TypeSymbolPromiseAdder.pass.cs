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

    private IFixedSet<To.NamespaceMemberDeclaration> TransformNamespaceMemberDeclarations(IEnumerable<From.NamespaceMemberDeclaration> from, IPromise<Symbol>? containingSymbol)
        => from.Select(f => TransformNamespaceMemberDeclaration(f, containingSymbol)).ToFixedSet();

    private To.NamespaceMemberDeclaration TransformNamespaceMemberDeclaration(From.NamespaceMemberDeclaration from, IPromise<Symbol>? containingSymbol)
        => from switch
        {
            From.TypeDeclaration f => TransformTypeDeclaration(f, containingSymbol),
            From.FunctionDeclaration f => TransformFunctionDeclaration(f),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private IFixedList<To.ClassMemberDeclaration> TransformClassMemberDeclarations(IEnumerable<From.ClassMemberDeclaration> from, IPromise<Symbol>? containingSymbol)
        => from.Select(f => TransformClassMemberDeclaration(f, containingSymbol)).ToFixedList();

    private To.ClassMemberDeclaration TransformClassMemberDeclaration(From.ClassMemberDeclaration from, IPromise<Symbol>? containingSymbol)
        => from switch
        {
            From.TypeDeclaration f => TransformTypeDeclaration(f, containingSymbol),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private IFixedList<To.StructMemberDeclaration> TransformStructMemberDeclarations(IEnumerable<From.StructMemberDeclaration> from, IPromise<Symbol>? containingSymbol)
        => from.Select(f => TransformStructMemberDeclaration(f, containingSymbol)).ToFixedList();

    private To.StructMemberDeclaration TransformStructMemberDeclaration(From.StructMemberDeclaration from, IPromise<Symbol>? containingSymbol)
        => from switch
        {
            From.TypeDeclaration f => TransformTypeDeclaration(f, containingSymbol),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private IFixedList<To.TraitMemberDeclaration> TransformTraitMemberDeclarations(IEnumerable<From.TraitMemberDeclaration> from, IPromise<Symbol>? containingSymbol)
        => from.Select(f => TransformTraitMemberDeclaration(f, containingSymbol)).ToFixedList();

    private To.TraitMemberDeclaration TransformTraitMemberDeclaration(From.TraitMemberDeclaration from, IPromise<Symbol>? containingSymbol)
        => from switch
        {
            From.TypeDeclaration f => TransformTypeDeclaration(f, containingSymbol),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.Package Create(From.Package from, IEnumerable<To.NamespaceMemberDeclaration> declarations, IEnumerable<To.NamespaceMemberDeclaration> testingDeclarations)
        => To.Package.Create(declarations, testingDeclarations, from.LexicalScope, from.Syntax, from.Symbol, from.References);

    private To.ClassDeclaration Create(From.ClassDeclaration from, IEnumerable<To.ClassMemberDeclaration> members, AcyclicPromise<UserTypeSymbol> symbol, IPromise<Symbol> containingSymbolPromise)
        => To.ClassDeclaration.Create(from.Syntax, from.IsAbstract, from.BaseTypeName, members, symbol, containingSymbolPromise, from.NewScope, from.GenericParameters, from.SupertypeNames, from.File, from.ContainingScope, from.ContainingNamespace);

    private To.StructDeclaration Create(From.StructDeclaration from, IEnumerable<To.StructMemberDeclaration> members, AcyclicPromise<UserTypeSymbol> symbol, IPromise<Symbol> containingSymbolPromise)
        => To.StructDeclaration.Create(from.Syntax, members, symbol, containingSymbolPromise, from.NewScope, from.GenericParameters, from.SupertypeNames, from.File, from.ContainingScope, from.ContainingNamespace);

    private To.TraitDeclaration Create(From.TraitDeclaration from, IEnumerable<To.TraitMemberDeclaration> members, AcyclicPromise<UserTypeSymbol> symbol, IPromise<Symbol> containingSymbolPromise)
        => To.TraitDeclaration.Create(from.Syntax, members, symbol, containingSymbolPromise, from.NewScope, from.GenericParameters, from.SupertypeNames, from.File, from.ContainingScope, from.ContainingNamespace);

}
