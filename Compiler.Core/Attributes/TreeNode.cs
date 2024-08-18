namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

public abstract class TreeNode : IChildTreeNode
{
    protected virtual bool InFinalTree { get; private set; }
    bool ITreeNode.InFinalTree => InFinalTree;

    /// <remarks>Volatile write not necessary because an out-of-order read is not an issue since it
    /// will just re-figure out the fact that the node is final. Does not check the invariant that
    /// the parent is in the final tree because that would probably require a volatile read and that
    /// volatile was used in other places too.</remarks>
    protected void MarkInFinalTree() => InFinalTree = true;
    void ITreeNode.MarkInFinalTree() => MarkInFinalTree();

    protected virtual bool MayHaveRewrite => false;
    bool IChildTreeNode.MayHaveRewrite => MayHaveRewrite;

    protected virtual ITreeNode? PeekParent() => null;
    ITreeNode? ITreeNode.PeekParent() => PeekParent();

    protected virtual IChildTreeNode? Rewrite()
        => MayHaveRewrite ? this : throw Child.RewriteNotSupported(this);
    IChildTreeNode? IChildTreeNode.Rewrite() => Rewrite();

    protected TreeNode() { }

    protected TreeNode(bool inFinalTree)
    {
        InFinalTree = inFinalTree;
    }
}
