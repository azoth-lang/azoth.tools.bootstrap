using Azoth.Tools.Bootstrap.Compiler.Core;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Instructions
{
    [Closed(
        typeof(CallInstruction),
        typeof(CallVirtualInstruction),
        typeof(InstructionWithResult))]
    public abstract class Instruction
    {
        public TextSpan Span { get; }
        public Scope Scope { get; }

        protected Instruction(TextSpan span, Scope scope)
        {
            Span = span;
            Scope = scope;
        }

        public override string ToString()
        {
            return ToInstructionString() + ContextCommentString();
        }

        public abstract string ToInstructionString();

        public virtual string ContextCommentString()
        {
            return $" // at {Span} in {Scope}";
        }
    }
}
