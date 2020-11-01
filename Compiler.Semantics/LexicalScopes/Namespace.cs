using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes
{
    internal class Namespace
    {
        public NamespaceName Name { get; }
        public FixedDictionary<TypeName, FixedSet<IPromise<Symbol>>> Symbols { get; }
        public FixedDictionary<TypeName, FixedSet<IPromise<Symbol>>> NestedSymbols { get; }
        public FixedDictionary<TypeName, FixedSet<IPromise<Symbol>>> SymbolsInPackage { get; }
        public FixedDictionary<TypeName, FixedSet<IPromise<Symbol>>> NestedSymbolsInPackage { get; }

        public Namespace(
            NamespaceName name,
            FixedDictionary<TypeName, FixedSet<IPromise<Symbol>>> symbols,
            FixedDictionary<TypeName, FixedSet<IPromise<Symbol>>> nestedSymbols,
            FixedDictionary<TypeName, FixedSet<IPromise<Symbol>>> symbolsInPackage,
            FixedDictionary<TypeName, FixedSet<IPromise<Symbol>>> nestedSymbolsInPackage)
        {
            Name = name;
            Symbols = symbols;
            NestedSymbols = nestedSymbols;
            SymbolsInPackage = symbolsInPackage;
            NestedSymbolsInPackage = nestedSymbolsInPackage;
        }
    }
}
