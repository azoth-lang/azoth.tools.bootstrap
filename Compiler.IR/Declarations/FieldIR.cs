using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.IR.Declarations
{
    public class FieldIR : MemberIR
    {
        public bool IsMutableBinding => Symbol.IsMutableBinding;
        public DataType DataType => Symbol.DataType;
        public new FieldSymbol Symbol { get; }

        public FieldIR(FieldSymbol symbol)
            : base(symbol)
        {
            Symbol = symbol;
        }
    }
}
