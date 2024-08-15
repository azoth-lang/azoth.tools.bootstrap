using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

internal sealed class PackageSymbols : IPackageSymbols
{
    public PackageSymbol PackageSymbol { get; }
    public FixedSymbolTree SymbolTree { get; }
    public FixedSymbolTree TestingSymbolTree { get; }

    public PackageSymbols(PackageSymbol packageSymbol, FixedSymbolTree symbolTree, FixedSymbolTree testingSymbolTree)
    {
        PackageSymbol = packageSymbol;
        SymbolTree = symbolTree;
        TestingSymbolTree = testingSymbolTree;
    }
}
