namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

/// <summary>
/// A child node in a syntax tree.
/// </summary>
public interface IChild
{
    protected internal bool MayHaveRewrite { get; }
}

/// <summary>
/// A child node in a syntax tree.
/// </summary>
public interface IChild<in TParent> : IChild
{
    protected internal void AttachParent(TParent parent);
}

public interface IRewritableChild<out TSelf> : IChild
{
    protected internal TSelf Rewrite();
}

public interface IChild<out TSelf, in TParent> : IChild<TParent>, IRewritableChild<TSelf>
    where TSelf : IChild<TSelf, TParent>
{
}
