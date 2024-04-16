using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Types.Scopes;

/// <summary>
/// A wrapper around <see cref="PackageReferenceScope"/> that provides a view on lexical scopes that
/// contains promises for symbols in the current tree.
/// </summary>
internal class PromiseScopeTree
{
    private readonly PackageReferenceScope rootScope;
    private readonly ISymbolTreeBuilder packageSymbols;

    public PromiseScopeTree(PackageReferenceScope rootScope, ISymbolTreeBuilder packageSymbols)
    {
        this.rootScope = rootScope;
        this.packageSymbols = packageSymbols;
    }

    public void AddPromise(NamespaceSymbol containingNamespace, StandardName name, IPromise<Symbol> symbolPromise)
    {
        // TODO
    }

    public void AddPromise(IPromise<Symbol> containingSymbol, StandardName name, IPromise<Symbol> symbolPromise)
    {
        // TODO
    }
}
