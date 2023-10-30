using System;
using System.Collections.Generic;
using System.Linq;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

/// <summary>
/// Symbol tree build for the testing symbols.
/// </summary>
public class TestingSymbolTreeBuilder : ISymbolTreeBuilder
{
    private readonly SymbolTreeBuilder treeBuilder;
    private readonly SymbolTreeBuilder testTreeBuilder;

    public PackageSymbol Package => treeBuilder.Package!;
    public IEnumerable<Symbol> Symbols => treeBuilder.Symbols.Concat(testTreeBuilder.Symbols);

    public TestingSymbolTreeBuilder(SymbolTreeBuilder treeBuilder)
    {
        if (treeBuilder.Package is null)
            throw new ArgumentException("Cannot be primitives tree", nameof(treeBuilder));
        this.treeBuilder = treeBuilder;
        testTreeBuilder = new SymbolTreeBuilder(treeBuilder.Package);
    }

    public bool Contains(Symbol symbol)
        => treeBuilder.Contains(symbol) || testTreeBuilder.Contains(symbol);

    public IEnumerable<Symbol> Children(Symbol symbol)
        => treeBuilder.Children(symbol).Concat(testTreeBuilder.Children(symbol));

    public void Add(Symbol symbol)
    {
        if (treeBuilder.Contains(symbol))
            return;
        testTreeBuilder.Add(symbol);
    }
}
