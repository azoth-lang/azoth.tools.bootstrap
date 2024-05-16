using System;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal abstract class ReferencedSymbolNode : IDeclarationNode
{
    public ISyntax? Syntax => null;

    public abstract Symbol Symbol { get; }

    internal virtual IPackageDeclarationNode InheritedPackage(IChildDeclarationNode caller, IChildDeclarationNode child)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedPackage), caller, child));

    internal virtual ISymbolTree InheritedSymbolTree(IChildDeclarationNode caller, IChildDeclarationNode child)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedSymbolTree), caller, child));

    internal virtual IPackageFacetDeclarationNode InheritedFacet(IChildDeclarationNode caller, IChildDeclarationNode child)
        => throw new NotImplementedException(
            Child.InheritFailedMessage(nameof(InheritedFacet), caller, child));
}
