using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage
{
    public class FieldIL : DeclarationIL
    {
        public bool IsMutableBinding => Symbol.IsMutableBinding;
        public DataType DataType => Symbol.DataType;
        public new FieldSymbol Symbol { get; }

        public FieldIL(FieldSymbol symbol)
            : base(true, symbol)
        {
            Symbol = symbol;
        }
    }
}
