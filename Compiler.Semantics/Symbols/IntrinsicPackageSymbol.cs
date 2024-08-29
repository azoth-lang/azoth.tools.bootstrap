using Azoth.Tools.Bootstrap.Compiler.Primitives;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

internal class IntrinsicPackageSymbol : IPackageSymbols
{
    #region Singleton
    public static IntrinsicPackageSymbol Instance { get; } = new IntrinsicPackageSymbol();

    private IntrinsicPackageSymbol() { }
    #endregion

    public PackageSymbol PackageSymbol => Intrinsic.Package;
    public FixedSymbolTree SymbolTree => Intrinsic.SymbolTree;
    public FixedSymbolTree TestingSymbolTree { get; }
        = new(Intrinsic.Package, FixedDictionary<Symbol, IFixedSet<Symbol>>.Empty);
}
