using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

/// <summary>
/// Symbol tree build for the testing symbols.
/// </summary>
public class TestingSymbolTreeBuilder : ISymbolTreeBuilder
{
    private readonly SymbolTreeBuilder treeBuilder;
    private readonly SymbolTreeBuilder testTreeBuilder;

    public PackageSymbol Package => treeBuilder.Package!;
    public IEnumerable<Symbol> Symbols => treeBuilder.Symbols.Concat(testTreeBuilder.Symbols).Distinct();

    public TestingSymbolTreeBuilder(SymbolTreeBuilder treeBuilder)
    {
        if (treeBuilder.Package is null)
            throw new ArgumentException("Cannot be primitives tree", nameof(treeBuilder));
        this.treeBuilder = treeBuilder;
        testTreeBuilder = new SymbolTreeBuilder(treeBuilder.Package);
    }

    public bool Contains(Symbol symbol)
        => treeBuilder.Contains(symbol) || testTreeBuilder.Contains(symbol);

    public IEnumerable<Symbol> GetChildrenOf(Symbol symbol)
        => treeBuilder.GetChildrenOf(symbol).Concat(testTreeBuilder.GetChildrenOf(symbol)).Distinct();

    public void Add(Symbol symbol)
    {
        if (treeBuilder.Contains(symbol))
            return;
        testTreeBuilder.Add(symbol);
    }

    public void AddInherited(TypeSymbol symbol, Symbol inheritedSymbol)
    {
        Add(symbol);
        if (!GetChildrenOf(symbol).Contains(inheritedSymbol))
            testTreeBuilder.AddInherited(symbol, inheritedSymbol);
    }

    public FixedSymbolTree Build()
        => new(Package, Symbols.ToFixedDictionary(s => s, s => GetChildrenOf(s).ToFixedSet()));
}
