namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

public interface IParent
{
    /// <summary>
    /// Whether this node is in its final state.
    /// </summary>
    /// <remarks>The final portion of the tree starts at the root and grows downward. No node can
    /// be final unless its parent is final. Even if a node has no rewrite rules, it still may not
    /// be final because to its children it is still not final since an ancestor could rewrite the
    /// subtree and replace the node.</remarks>
    protected internal bool IsFinal { get; }
}

/// <summary>
/// A child node in a syntax tree.
/// </summary>
public interface IChild : IParent
{
    protected internal bool MayHaveRewrite { get; }

    /// <summary>
    /// Rewrite the child node.
    /// </summary>
    /// <returns>The node to replace this node with or <see langword="this"/> if this node is
    /// final. Returned type must inherit from the type used with <see cref="Child{T}"/>.</returns>
    protected internal IChild? Rewrite();

    protected internal void MarkFinal();
}

/// <summary>
/// A child node in a syntax tree.
/// </summary>
public interface IChild<in TParent> : IChild
{
    protected internal void SetParent(TParent parent);
}
