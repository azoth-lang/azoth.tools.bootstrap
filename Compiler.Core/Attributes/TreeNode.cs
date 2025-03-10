using System;
using System.Diagnostics;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

public abstract class TreeNode : IChildTreeNode
{
    protected bool InFinalTree { [DebuggerStepThrough] get; [DebuggerStepThrough] private set; }
    bool ITreeNode.InFinalTree { [DebuggerStepThrough] get => InFinalTree; }

    /// <remarks>Volatile write not necessary because an out-of-order read is not an issue since it
    /// will just re-figure out the fact that the node is final. Does not check the invariant that
    /// the parent is in the final tree because that would probably require a volatile read and for
    /// volatile to be used in other places too.</remarks>
    protected void MarkInFinalTree()
    {
#if DEBUG
        // Ensure that nodes are marked final only from the root downward
        if (InFinalTree) return;
        if (PeekParent()?.InFinalTree != true)
            throw new InvalidOperationException("Cannot mark node final when parent node isn't final");
#endif
        InFinalTree = true;
    }

    void ITreeNode.MarkInFinalTree() => MarkInFinalTree();

    protected virtual bool MayHaveRewrite { [DebuggerStepThrough] get => false; }
    bool IChildTreeNode.MayHaveRewrite { [DebuggerStepThrough] get => MayHaveRewrite; }

    protected virtual ITreeNode? PeekParent() => null;
    [DebuggerStepThrough]
    ITreeNode? ITreeNode.PeekParent() => PeekParent();

    protected virtual IChildTreeNode Rewrite()
        => MayHaveRewrite ? this : throw Child.RewriteNotSupported(this);
    [DebuggerStepThrough]
    IChildTreeNode IChildTreeNode.Rewrite() => Rewrite();

    protected TreeNode() { }

    protected TreeNode(bool inFinalTree)
    {
        InFinalTree = inFinalTree;
    }
}
