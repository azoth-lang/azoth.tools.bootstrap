using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage
{
    public sealed class NamedParameterIL : ParameterIL
    {
        public new VariableSymbol Symbol { get; }

        public NamedParameterIL(VariableSymbol symbol)
            : base(symbol, symbol.IsMutableBinding, symbol.DataType)
        {
            Symbol = symbol;
        }
    }
}
