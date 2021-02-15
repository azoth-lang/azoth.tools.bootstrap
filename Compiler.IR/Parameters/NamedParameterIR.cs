using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.IL;

namespace Azoth.Tools.Bootstrap.Compiler.IR.Parameters
{
    public class NamedParameterIR : ConstructorParameterIR
    {
        public VariableSymbol Symbol { get; }
        public bool IsMutableBinding => Symbol.IsMutableBinding;
        public DataType DataType => Symbol.DataType;

        public NamedParameterIR(VariableSymbol symbol)
        {
            Symbol = symbol;
        }

        public NamedParameterIL ToIL()
        {
            return new NamedParameterIL();
        }
    }
}
