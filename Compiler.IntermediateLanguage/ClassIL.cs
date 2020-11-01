using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage
{
    public class ClassIL : DeclarationIL
    {
        public FixedList<DeclarationIL> Members { get; }
        public new ObjectTypeSymbol Symbol { get; }

        public ClassIL(ObjectTypeSymbol symbol, FixedList<DeclarationIL> members)
            : base(false, symbol)
        {
            Symbol = symbol;
            Members = members;
        }
    }
}
