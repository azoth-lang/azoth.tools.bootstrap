using System;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal abstract class ReferencedSymbolNode : ISymbolNode
{
    public abstract Symbol Symbol { get; }

    internal virtual IPackageSymbolNode InheritedPackage(IChildSymbolNode caller, IChildSymbolNode child)
        => throw new NotImplementedException(
            $"{nameof(InheritedPackage)} not implemented for child node type {child.GetType().Name}.");

    internal virtual ISymbolTree InheritedSymbolTree(IChildSymbolNode caller, IChildSymbolNode child)
        => throw new NotImplementedException(
            $"{nameof(InheritedSymbolTree)} not implemented for child node type {child.GetType().Name}.");

}
