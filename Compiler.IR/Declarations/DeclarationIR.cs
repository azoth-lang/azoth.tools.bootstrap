using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.IR.Declarations
{
    public abstract class DeclarationIR
    {
        public Symbol Symbol { get; }

        protected DeclarationIR(Symbol symbol)
        {
            Symbol = symbol;
        }

        public override string ToString()
        {
            return Symbol.ToILString();
        }
    }
}
