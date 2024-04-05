namespace Azoth.Tools.Bootstrap.Compiler.IST;

public interface IPass<TFrom, TContextIn, TTo, TContextOut>
{
    public static abstract (TTo, TContextOut) Run(TFrom from, TContextIn context);
}
