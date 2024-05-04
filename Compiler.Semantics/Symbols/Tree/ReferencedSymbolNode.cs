using System;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal abstract class ReferencedSymbolNode : ISymbolNode
{
    public abstract Symbol Symbol { get; }

    internal virtual IPackageSymbolNode InheritedPackage(IChildSymbolNode caller, IChildSymbolNode child)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedPackage), caller, child));

    internal virtual ISymbolTree InheritedSymbolTree(IChildSymbolNode caller, IChildSymbolNode child)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedSymbolTree), caller, child));

    internal virtual IPackageFacetSymbolNode InheritedFacet(IChildSymbolNode caller, IChildSymbolNode child)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedFacet), caller, child));
}
