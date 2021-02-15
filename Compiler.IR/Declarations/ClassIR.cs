using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.IR.Declarations
{
    public class ClassIR : DeclarationIR
    {
        public new ObjectTypeSymbol Symbol { get; }
        public FixedList<MemberIR> Members { get; }

        public ClassIR(ObjectTypeSymbol symbol, FixedList<MemberIR> members)
            : base(symbol)
        {
            Symbol = symbol;
            Members = members;
        }
    }
}
