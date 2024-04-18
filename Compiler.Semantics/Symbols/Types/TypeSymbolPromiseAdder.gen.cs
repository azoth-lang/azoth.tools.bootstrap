using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using From = Azoth.Tools.Bootstrap.Compiler.IST.WithoutCompilationUnits;
using To = Azoth.Tools.Bootstrap.Compiler.IST.WithTypeDeclarationPromises;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Types;

internal sealed partial class TypeSymbolPromiseAdder
{
    partial void StartRun();

    partial void EndRun(To.Package package);

    private partial To.Package Transform(From.Package from);

    private To.Package Create(From.Package from, IPromise<Symbol>? childContainingSymbol)
        => Create(from, Transform(from.Declarations, childContainingSymbol),
            Transform(from.TestingDeclarations, childContainingSymbol));

    private To.Package Create(From.Package from,
        IEnumerable<To.NamespaceMemberDeclaration> declarations,
        IEnumerable<To.NamespaceMemberDeclaration> testingDeclarations)
        => To.Package.Create(declarations, testingDeclarations, from.LexicalScope, from.Syntax, from.Symbol, from.References);

    private IFixedSet<To.NamespaceMemberDeclaration> Transform(
        IEnumerable<From.NamespaceMemberDeclaration> from,
        IPromise<Symbol>? containingSymbol)
        => from.Select(f => Transform(f, containingSymbol)).ToFixedSet();

    private To.NamespaceMemberDeclaration Transform(From.NamespaceMemberDeclaration from, IPromise<Symbol>? containingSymbol)
        => from switch
        {
            From.TypeDeclaration f => Transform(f, containingSymbol),
            From.FunctionDeclaration f => f,
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private partial To.TypeDeclaration Transform(From.TypeDeclaration from, IPromise<Symbol>? containingSymbol);

    private To.TypeDeclaration Create(From.TypeDeclaration from,
        AcyclicPromise<UserTypeSymbol> symbol,
        IPromise<Symbol> containingSymbol,
        IPromise<Symbol>? childContainingSymbol)
        => from switch
        {
            From.ClassDeclaration f => Create(f, symbol, containingSymbol, childContainingSymbol),
            From.TraitDeclaration f => Create(f, symbol, containingSymbol, childContainingSymbol),
            From.StructDeclaration f => Create(f, symbol, containingSymbol, childContainingSymbol),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.ClassDeclaration Create(From.ClassDeclaration from,
        AcyclicPromise<UserTypeSymbol> symbol,
        IPromise<Symbol> containingSymbol,
        IPromise<Symbol>? childContainingSymbol)
        => Create(from, Transform(from.Members, childContainingSymbol), symbol, containingSymbol);

    private To.ClassDeclaration Create(
        From.ClassDeclaration from,
        IEnumerable<To.ClassMemberDeclaration> members,
        AcyclicPromise<UserTypeSymbol> symbol,
        IPromise<Symbol> containingSymbol)
        => To.ClassDeclaration.Create(from.Syntax, from.IsAbstract, from.BaseTypeName, members, symbol, containingSymbol,
            from.LexicalScope, from.GenericParameters, from.SupertypeNames, from.File, from.ContainingLexicalScope,
            from.ContainingSymbol);

    private IFixedList<To.ClassMemberDeclaration> Transform(
        IEnumerable<From.ClassMemberDeclaration> from,
        IPromise<Symbol>? containingSymbol)
        => from.Select(f => Transform(f, containingSymbol)).ToFixedList();

    private To.ClassMemberDeclaration Transform(
        From.ClassMemberDeclaration from, IPromise<Symbol>? containingSymbol)
        => from switch
        {
            From.ClassDeclaration f => Transform(f, containingSymbol),
            From.TraitDeclaration f => Transform(f, containingSymbol),
            From.StructDeclaration f => Transform(f, containingSymbol),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.TraitDeclaration Create(From.TraitDeclaration from,
        AcyclicPromise<UserTypeSymbol> symbol,
        IPromise<Symbol> containingSymbol,
        IPromise<Symbol>? childContainingSymbol)
        => Create(from, Transform(from.Members, childContainingSymbol), symbol, containingSymbol);

    private To.TraitDeclaration Create(
        From.TraitDeclaration from,
        IEnumerable<To.TraitMemberDeclaration> members,
        AcyclicPromise<UserTypeSymbol> symbol,
        IPromise<Symbol> containingSymbol)
        => To.TraitDeclaration.Create(from.Syntax, members, symbol, containingSymbol, from.LexicalScope, from.GenericParameters, from.SupertypeNames, from.File, from.ContainingLexicalScope, from.ContainingSymbol);

    private IFixedList<To.TraitMemberDeclaration> Transform(
        IEnumerable<From.TraitMemberDeclaration> from,
        IPromise<Symbol>? containingSymbol)
        => from.Select(f => Transform(f, containingSymbol)).ToFixedList();

    private To.TraitMemberDeclaration Transform(From.TraitMemberDeclaration from, IPromise<Symbol>? containingSymbol)
        => from switch
        {
            From.ClassDeclaration f => Transform(f, containingSymbol),
            From.TraitDeclaration f => Transform(f, containingSymbol),
            From.StructDeclaration f => Transform(f, containingSymbol),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.StructDeclaration Create(
        From.StructDeclaration from,
        AcyclicPromise<UserTypeSymbol> symbol,
        IPromise<Symbol> containingSymbol,
        IPromise<Symbol>? childContainingSymbol)
        => Create(from, Transform(from.Members, childContainingSymbol), symbol, containingSymbol);

    private To.StructDeclaration Create(From.StructDeclaration from,
        IEnumerable<To.StructMemberDeclaration> members,
        AcyclicPromise<UserTypeSymbol> symbol,
        IPromise<Symbol> containingSymbol)
        => To.StructDeclaration.Create(from.Syntax, members, symbol, containingSymbol, from.LexicalScope, from.GenericParameters, from.SupertypeNames, from.File, from.ContainingLexicalScope, from.ContainingSymbol);

    private IFixedList<To.StructMemberDeclaration> Transform(
        IEnumerable<From.StructMemberDeclaration> from,
        IPromise<Symbol>? containingSymbol)
        => from.Select(f => Transform(f, containingSymbol)).ToFixedList();

    private To.StructMemberDeclaration Transform(
        From.StructMemberDeclaration from,
        IPromise<Symbol>? containingSymbol)
        => from switch
        {
            From.ClassDeclaration f => Transform(f, containingSymbol),
            From.TraitDeclaration f => Transform(f, containingSymbol),
            From.StructDeclaration f => Transform(f, containingSymbol),
            _ => throw ExhaustiveMatch.Failed(from),
        };
}
