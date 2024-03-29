using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

public class SymbolForest
{
    public PrimitiveSymbolTree PrimitiveSymbolTree { get; }
    private readonly FixedDictionary<PackageSymbol, ISymbolTree> packageTrees;
    public IEnumerable<PackageSymbol> Packages => packageTrees.Keys;

    public IEnumerable<Symbol> GlobalSymbols => PrimitiveSymbolTree.GlobalSymbols
                                                                   .Concat(Packages.SelectMany(Children));
    public IEnumerable<Symbol> Symbols => packageTrees.Values.SelectMany(t => t.Symbols);

    public SymbolForest(PrimitiveSymbolTree primitiveSymbolTree, FixedSymbolTree intrinsicSymbolTree,
        ISymbolTreeBuilder symbolTreeBuilder, IEnumerable<FixedSymbolTree> packageTrees)
    {
        if (symbolTreeBuilder.Package is null)
            throw new ArgumentException("Can't be builder for primitive symbols", nameof(symbolTreeBuilder));
        PrimitiveSymbolTree = primitiveSymbolTree;
        this.packageTrees = packageTrees.Append<ISymbolTree>(symbolTreeBuilder).Append(intrinsicSymbolTree).ToFixedDictionary(t => t.Package!);
    }

    public SymbolForest(PrimitiveSymbolTree primitiveSymbolTree, FixedSymbolTree intrinsicSymbolTree, IEnumerable<FixedSymbolTree> packageTrees)
    {
        PrimitiveSymbolTree = primitiveSymbolTree;
        this.packageTrees = packageTrees.Append(intrinsicSymbolTree).ToFixedDictionary(t => t.Package!, t => (ISymbolTree)t);
    }

    public IEnumerable<Symbol> Children(Symbol symbol)
    {
        if (symbol.Package is null)
            return PrimitiveSymbolTree.GetChildrenOf(symbol);

        if (!packageTrees.TryGetValue(symbol.Package, out var tree))
            throw new ArgumentException("Symbol must be for one of the packages in this tree", nameof(symbol));

        return tree.GetChildrenOf(symbol);
    }
}
