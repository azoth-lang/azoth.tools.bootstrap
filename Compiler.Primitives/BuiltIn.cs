using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

namespace Azoth.Tools.Bootstrap.Compiler.Primitives;

public static class BuiltIn
{
    public static SymbolForest CreateSymbolForest(IEnumerable<FixedSymbolTree> packageTrees)
        => new(Primitive.SymbolTree,  packageTrees);

    public static SymbolForest CreateSymbolForest(ISymbolTreeBuilder symbolTree, IEnumerable<FixedSymbolTree> packageTrees)
        => new(Primitive.SymbolTree, symbolTree, packageTrees);
}
