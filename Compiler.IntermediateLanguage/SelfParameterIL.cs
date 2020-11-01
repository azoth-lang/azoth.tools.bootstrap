using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage
{
    public sealed class SelfParameterIL : ParameterIL
    {
        public new SelfParameterSymbol Symbol { get; }

        public SelfParameterIL(SelfParameterSymbol symbol)
            : base(symbol, symbol.IsMutableBinding, symbol.DataType)
        {
            Symbol = symbol;
        }
    }
}
