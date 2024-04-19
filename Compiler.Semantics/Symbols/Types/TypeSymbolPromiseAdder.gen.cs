using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using ExhaustiveMatching;
using From = Azoth.Tools.Bootstrap.Compiler.IST.WithoutCompilationUnits;
using To = Azoth.Tools.Bootstrap.Compiler.IST.WithTypeDeclarationPromises;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Types;

internal sealed partial class TypeSymbolPromiseAdder
{
    private To.Package Create(From.Package from, IPromise<Symbol>? childContainingSymbol)
        => Create(from, Transform(from.Declarations, childContainingSymbol),
            Transform(from.TestingDeclarations, childContainingSymbol));

    private To.NamespaceMemberDeclaration Transform(From.NamespaceMemberDeclaration from, IPromise<Symbol>? containingSymbol)
        => from switch
        {
            From.TypeDeclaration f => Transform(f, containingSymbol),
            From.FunctionDeclaration f => f,
            _ => throw ExhaustiveMatch.Failed(from),
        };

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
