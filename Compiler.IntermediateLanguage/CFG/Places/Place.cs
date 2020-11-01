using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Operands;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Places
{
    [Closed(
        typeof(VariablePlace),
        typeof(FieldPlace))]
    public abstract class Place
    {
        public TextSpan Span { get; }

        protected Place(TextSpan span)
        {
            Span = span;
        }

        public abstract Operand ToOperand(TextSpan span);

        public abstract override string ToString();
    }
}
