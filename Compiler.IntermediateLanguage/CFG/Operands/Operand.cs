using Azoth.Tools.Bootstrap.Compiler.Core;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Operands
{
    [Closed(
        typeof(VariableReference))]
    public abstract class Operand
    {
        public TextSpan Span { get; }

        protected Operand(in TextSpan span)
        {
            Span = span;
        }
    }
}
