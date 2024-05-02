namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

/// <summary>
/// A child node in a syntax tree.
/// </summary>
public interface IChild<in TParent>
{
    protected internal void AttachParent(TParent parent);
}
