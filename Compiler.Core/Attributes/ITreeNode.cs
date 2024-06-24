namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

public interface ITreeNode
{
    /// <summary>
    /// Peek at this nodes parent in the tree. This <i>does not count as traversing the reference</i>
    /// and <b>must not be used</b> when computing attribute values.
    /// </summary>
    /// <remarks>This exists so that the internals of the cyclic attribute evaluation system can
    /// follow the path from a node upward to the root of the tree to determine if a given parent
    /// traversal is under a rewrite.</remarks>
    protected internal ITreeNode? PeekParent();
}
