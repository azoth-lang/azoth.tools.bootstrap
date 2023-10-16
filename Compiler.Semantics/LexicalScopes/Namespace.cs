using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

internal class Namespace
{
    public NamespaceName Name { get; }
    public FixedDictionary<Name, FixedSet<IPromise<Symbol>>> Symbols { get; }
    public FixedDictionary<Name, FixedSet<IPromise<Symbol>>> NestedSymbols { get; }
    public FixedDictionary<Name, FixedSet<IPromise<Symbol>>> SymbolsInPackage { get; }
    public FixedDictionary<Name, FixedSet<IPromise<Symbol>>> NestedSymbolsInPackage { get; }

    public Namespace(
        NamespaceName name,
        FixedDictionary<Name, FixedSet<IPromise<Symbol>>> symbols,
        FixedDictionary<Name, FixedSet<IPromise<Symbol>>> nestedSymbols,
        FixedDictionary<Name, FixedSet<IPromise<Symbol>>> symbolsInPackage,
        FixedDictionary<Name, FixedSet<IPromise<Symbol>>> nestedSymbolsInPackage)
    {
        Name = name;
        Symbols = symbols;
        NestedSymbols = nestedSymbols;
        SymbolsInPackage = symbolsInPackage;
        NestedSymbolsInPackage = nestedSymbolsInPackage;
    }
}
