namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

/// <summary>
/// A child node in a syntax tree.
/// </summary>
public interface IChild
{
    protected internal bool MayHaveRewrite { get; }

    protected internal bool IsFinal { get; }

    /// <summary>
    /// Rewrite the child node.
    /// </summary>
    /// <returns>The node to replace this node with or <see langword="null"/> if this node is
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
