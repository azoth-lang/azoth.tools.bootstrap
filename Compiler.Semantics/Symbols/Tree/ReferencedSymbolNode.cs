using System;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal abstract class ReferencedSymbolNode : ISymbolNode
{
    public abstract Symbol Symbol { get; }

    internal virtual IPackageSymbolNode InheritedPackage(IChildSymbolNode caller, IChildSymbolNode child)
        => throw new NotImplementedException(
            $"{nameof(InheritedPackage)} not implemented for child node type {child.GetType().GetFriendlyName()}.");

    internal virtual ISymbolTree InheritedSymbolTree(IChildSymbolNode caller, IChildSymbolNode child)
        => throw new NotImplementedException(
            $"{nameof(InheritedSymbolTree)} not implemented for child node type {child.GetType().GetFriendlyName()}.");

    internal virtual INamespaceSymbolNode InheritedGlobalNamespace(IChildSymbolNode caller, IChildSymbolNode child)
        => throw new NotImplementedException(
            $"{nameof(InheritedGlobalNamespace)} not implemented for child node type {child.GetType().GetFriendlyName()}.");
}
