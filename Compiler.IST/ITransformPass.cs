namespace Azoth.Tools.Bootstrap.Compiler.IST;

/// <summary>
/// A compiler pass that transforms from one tree to another.
/// </summary>
public interface ITransformPass<TFrom, TContextIn, TTo, TContextOut>
{
    public static abstract (TTo, TContextOut) Run(TFrom from, TContextIn context);
}
