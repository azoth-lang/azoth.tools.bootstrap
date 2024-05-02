using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

public interface IPackageSymbols
{
    PackageSymbol PackageSymbol { get; }
    FixedSymbolTree SymbolTree { get; }
    FixedSymbolTree TestingSymbolTree { get; }
}
