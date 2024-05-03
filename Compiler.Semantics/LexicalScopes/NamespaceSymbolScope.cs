using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

internal class NamespaceSymbolScope
{
    public NamespaceName Name { get; }
    public FixedDictionary<TypeName, IFixedSet<IPromise<Symbol>>> Symbols { get; }
    public FixedDictionary<TypeName, IFixedSet<IPromise<Symbol>>> NestedSymbols { get; }
    public FixedDictionary<TypeName, IFixedSet<IPromise<Symbol>>> SymbolsInPackage { get; }
    public FixedDictionary<TypeName, IFixedSet<IPromise<Symbol>>> NestedSymbolsInPackage { get; }

    public NamespaceSymbolScope(
        NamespaceName name,
        FixedDictionary<TypeName, IFixedSet<IPromise<Symbol>>> symbols,
        FixedDictionary<TypeName, IFixedSet<IPromise<Symbol>>> nestedSymbols,
        FixedDictionary<TypeName, IFixedSet<IPromise<Symbol>>> symbolsInPackage,
        FixedDictionary<TypeName, IFixedSet<IPromise<Symbol>>> nestedSymbolsInPackage)
    {
        Name = name;
        Symbols = symbols;
        NestedSymbols = nestedSymbols;
        SymbolsInPackage = symbolsInPackage;
        NestedSymbolsInPackage = nestedSymbolsInPackage;
    }
}
