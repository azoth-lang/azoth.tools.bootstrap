namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

public interface ITreeNode
{
    /// <summary>
    /// Whether this node is/will be in the final tree. CAUTION: Not updated for every node. Only
    /// correct for nodes that are needed to determine if rewrites are complete.
    /// </summary>
    /// <remarks>The set of nodes that will be in the final tree starts at the root of the tree and
    /// grows downward from there. For a node to be in the final tree, it's parent must be in the
    /// final tree and it must be cached and hence no further rewrites possible. The reason the
    /// parent must be in the final tree is that any node above this node that can be rewritten
    /// could change the subtree under it to remove this node.</remarks>
    protected internal bool InFinalTree { get; }

    /// <summary>
    /// Mark this node as being in the final tree.
    /// </summary>
    protected internal void MarkInFinalTree();

    /// <summary>
    /// Peek at this nodes parent in the tree. This <i>does not count as traversing the reference</i>
    /// and <b>must not be used</b> when computing attribute values.
    /// </summary>
    /// <remarks>This exists so that the internals of the cyclic attribute evaluation system can
    /// follow the path from a node upward to the root of the tree to determine if a given parent
    /// traversal is under a rewrite.</remarks>
    protected internal ITreeNode? PeekParent();
}
