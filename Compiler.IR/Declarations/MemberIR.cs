using Azoth.Tools.Bootstrap.Compiler.Symbols;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.IR.Declarations
{
    [Closed(
        typeof(FieldIR),
        typeof(MethodIR))]
    public abstract class MemberIR : DeclarationIR
    {
        protected MemberIR(Symbol symbol)
            : base(symbol) { }
    }
}
