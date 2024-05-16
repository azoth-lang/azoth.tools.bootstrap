namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

/// <summary>
/// A child node in a syntax tree.
/// </summary>
public interface IChild
{
    protected internal bool MayHaveRewrite { get; }

    /// <summary>
    /// Rewrite the child node.
    /// </summary>
    /// <returns>Must return a type that implements the same interface the child is being accessed
    /// through.</returns>
    protected internal IChild Rewrite();
}

/// <summary>
/// A child node in a syntax tree.
/// </summary>
public interface IChild<in TParent> : IChild
{
    protected internal void AttachParent(TParent parent);
}
