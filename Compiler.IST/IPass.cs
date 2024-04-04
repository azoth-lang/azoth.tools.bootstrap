namespace Azoth.Tools.Bootstrap.Compiler.IST;

public interface IPass<in TContext, in TFrom, out TTo>
{
    public static abstract TTo Run(TContext context, TFrom from);

    protected TTo Run(TFrom from);
}
