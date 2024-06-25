namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

/// <summary>
/// A child node in a syntax tree.
/// </summary>
public interface IChildTreeNode : ITreeNode
{
    protected internal bool MayHaveRewrite { get; }

    /// <summary>
    /// Rewrite the child node.
    /// </summary>
    /// <returns>The node to replace this node with or <see langword="this"/> if this node is
    /// final. Returned type must inherit from the type used with <see cref="RewritableChild{T}"/>.</returns>
    protected internal IChildTreeNode? Rewrite();
}

/// <summary>
/// A child node in a syntax tree.
/// </summary>
public interface IChildTreeNode<in TParent> : IChildTreeNode
    where TParent : ITreeNode
{
    protected internal void SetParent(TParent parent);
}
