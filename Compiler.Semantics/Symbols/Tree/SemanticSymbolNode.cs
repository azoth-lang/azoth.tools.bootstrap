using System;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal abstract class SemanticSymbolNode : ISymbolNode
{
    public abstract Symbol Symbol { get; }

    public virtual IPackageSymbolNode InheritedPackage(IChildSymbolNode caller, IChildSymbolNode child)
        => throw new NotImplementedException($"{nameof(InheritedPackage)} not implemented for child node type {child.GetType().GetFriendlyName()}.");
}
