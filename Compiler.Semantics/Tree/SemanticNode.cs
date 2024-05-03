using System;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class SemanticNode : ISemanticNode
{
    public abstract ISyntax Syntax { get; }

    internal virtual ISymbolNode InheritedContainingSymbolNode(IChildNode caller, IChildNode child)
        => throw new NotImplementedException(
            $"{nameof(InheritedContainingSymbolNode)} not implemented for child node type {child.GetType().GetFriendlyName()}.");

    internal virtual IPackageNode InheritedPackage(IChildNode caller, IChildNode child)
        => throw new NotImplementedException(
            $"{nameof(InheritedPackage)} not implemented for child node type {child.GetType().GetFriendlyName()}.");

    internal virtual CodeFile InheritedFile(IChildNode caller, IChildNode child)
        => throw new NotImplementedException(
            $"{nameof(InheritedFile)} not implemented for child node type {child.GetType().GetFriendlyName()}.");
}
