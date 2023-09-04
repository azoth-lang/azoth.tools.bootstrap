using System.Collections.Generic;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

/// <summary>
/// A symbol tree is a immutable collection of symbols that answers the question:
/// For any given symbol, what are its child symbols.
/// </summary>
public interface ISymbolTree
{
    PackageSymbol? Package { get; }
    IEnumerable<Symbol> Symbols { get; }
    bool Contains(Symbol symbol);
    IEnumerable<Symbol> Children(Symbol symbol);
}
