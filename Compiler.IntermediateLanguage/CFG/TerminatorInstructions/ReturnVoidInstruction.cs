using Azoth.Tools.Bootstrap.Compiler.Core;

namespace Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.TerminatorInstructions
{
    public class ReturnVoidInstruction : TerminatorInstruction
    {
        public ReturnVoidInstruction(TextSpan span, Scope scope)
            : base(span, scope)

        {
        }

        public override string ToInstructionString()
        {
            return "RETURN";
        }
    }
}
