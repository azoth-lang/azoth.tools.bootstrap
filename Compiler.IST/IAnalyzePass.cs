namespace Azoth.Tools.Bootstrap.Compiler.IST;

public interface IAnalyzePass<TLang, TContextIn, TContextOut> : ITransformPass<TLang, TContextIn, TLang, TContextOut>
{
    public new static abstract TContextOut Run(TLang value, TContextIn context);
}
