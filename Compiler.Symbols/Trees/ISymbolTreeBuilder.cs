using System.Collections.Generic;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

public interface ISymbolTreeBuilder : ISymbolTree
{
    void Add(Symbol symbol);

    void Add(IEnumerable<Symbol> symbols)
    {
        foreach (var symbol in symbols)
            Add(symbol);
    }

    void AddInherited(TypeSymbol symbol, Symbol inheritedSymbol);

    FixedSymbolTree Build();
}
