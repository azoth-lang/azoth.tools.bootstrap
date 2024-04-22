using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using From = Azoth.Tools.Bootstrap.Compiler.IST.WithoutCompilationUnits;
using To = Azoth.Tools.Bootstrap.Compiler.IST.WithTypeDeclarationPromises;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Types;

internal partial class TypeSymbolPromiseAdder
{
    private partial To.Package Transform(From.Package from)
        => Create(from, childContainingSymbol: null);

    private partial To.TypeDeclaration Transform(From.TypeDeclaration from, IPromise<Symbol>? containingSymbol)
    {
        var typeSymbolPromise = new AcyclicPromise<UserTypeSymbol>();
        containingSymbol ??= Promise.ForValue(from.ContainingNamespace.Assigned());
        return Create(from, symbol: typeSymbolPromise, containingSymbol: containingSymbol,
            childContainingSymbol: typeSymbolPromise);
    }

    private To.FunctionDeclaration Create(From.FunctionDeclaration from)
        => from;
}
