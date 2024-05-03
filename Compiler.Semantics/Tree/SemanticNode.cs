using System;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class SemanticNode : ISemanticNode
{
    public abstract ISyntax Syntax { get; }

    public virtual Symbol InheritedContainingSymbol(IChildNode caller, IChildNode child)
        => throw new NotImplementedException($"{nameof(InheritedContainingSymbol)} not implemented for child node type {child.GetType().GetFriendlyName()}.");
}
