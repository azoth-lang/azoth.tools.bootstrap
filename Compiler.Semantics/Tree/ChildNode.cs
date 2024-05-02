using System;
using System.Threading;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class ChildNode : SemanticNode, IChild<ISemanticNode>
{
    private ISemanticNode? parent;
    public ISemanticNode Parent => parent ?? throw new InvalidOperationException("Parent is not set.");

    public void AttachParent(ISemanticNode parent)
    {
        var oldParent = Interlocked.CompareExchange(ref this.parent, parent, null);
        if (oldParent is not null) throw new InvalidOperationException("Parent is already set.");
    }
}
