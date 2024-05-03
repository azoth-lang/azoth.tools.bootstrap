using System;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class SemanticNode : ISemanticNode
{
    public abstract ISyntax Syntax { get; }

    public virtual NamespaceSymbol? InheritedContainingNamespace
        => throw new NotImplementedException($"{nameof(InheritedContainingNamespace)} not implemented for this node type.");
}
