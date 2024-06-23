namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

public interface IParent
{
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
}

/// <summary>
/// A child node in a syntax tree.
/// </summary>
public interface IChild<in TParent> : IChild
    where TParent : IParent
{
    protected internal void SetParent(TParent parent);
}
