using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using ExhaustiveMatching;
using From = Azoth.Tools.Bootstrap.Compiler.IST.WithoutCompilationUnits;
using To = Azoth.Tools.Bootstrap.Compiler.IST.WithTypeDeclarationPromises;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Types;

internal sealed partial class TypeSymbolPromiseAdder
{
    private To.Package Create(From.Package from, IPromise<Symbol>? childContainingSymbol)
        => Create(from, TransformNamespaceMemberDeclarations(from.Declarations, childContainingSymbol),
            TransformNamespaceMemberDeclarations(from.TestingDeclarations, childContainingSymbol));

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
        => Create(from, TransformClassMemberDeclarations(from.Members, childContainingSymbol), symbol, containingSymbol);

    private To.TraitDeclaration Create(From.TraitDeclaration from,
        AcyclicPromise<UserTypeSymbol> symbol,
        IPromise<Symbol> containingSymbol,
        IPromise<Symbol>? childContainingSymbol)
        => Create(from, TransformTraitMemberDeclarations(from.Members, childContainingSymbol), symbol, containingSymbol);

    private To.StructDeclaration Create(
        From.StructDeclaration from,
        AcyclicPromise<UserTypeSymbol> symbol,
        IPromise<Symbol> containingSymbol,
        IPromise<Symbol>? childContainingSymbol)
        => Create(from, TransformStructMemberDeclarations(from.Members, childContainingSymbol), symbol, containingSymbol);
}
