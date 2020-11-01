using Azoth.Tools.Bootstrap.Compiler.Core;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.TerminatorInstructions
{
    /// <summary>
    /// An instruction that terminates a block
    /// </summary>
    [Closed(
        typeof(GotoInstruction),
        typeof(IfInstruction),
        typeof(ReturnValueInstruction),
        typeof(ReturnVoidInstruction))]
    public abstract class TerminatorInstruction
    {
        public TextSpan Span { get; }
        public Scope Scope { get; }

        protected TerminatorInstruction(TextSpan span, Scope scope)
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
