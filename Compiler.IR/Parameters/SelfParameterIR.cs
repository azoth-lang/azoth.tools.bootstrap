using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.IR.Parameters
{
    public class SelfParameterIR : ParameterIR
    {
        public SelfParameterSymbol Symbol { get; }

        public SelfParameterIR(SelfParameterSymbol symbol)
        {
            Symbol = symbol;
        }
    }
}
