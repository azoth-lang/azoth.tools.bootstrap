using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using ExhaustiveMatching;
using From = Azoth.Tools.Bootstrap.Compiler.IST.WithoutCompilationUnits;
using To = Azoth.Tools.Bootstrap.Compiler.IST.WithTypeDeclarationPromises;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Types;

internal sealed partial class TypeSymbolPromiseAdder
{
    private To.TypeDeclaration CreateTypeDeclaration(From.TypeDeclaration from,
        AcyclicPromise<UserTypeSymbol> symbol,
        IPromise<Symbol> containingSymbol,
        IPromise<Symbol>? childContainingSymbol)
        => from switch
        {
            From.ClassDeclaration f => CreateClassDeclaration(f, symbol, containingSymbol, childContainingSymbol),
            From.TraitDeclaration f => CreateTraitDeclaration(f, symbol, containingSymbol, childContainingSymbol),
            From.StructDeclaration f => CreateStructDeclaration(f, symbol, containingSymbol, childContainingSymbol),
            _ => throw ExhaustiveMatch.Failed(from),
        };
}
