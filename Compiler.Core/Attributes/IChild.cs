namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

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
