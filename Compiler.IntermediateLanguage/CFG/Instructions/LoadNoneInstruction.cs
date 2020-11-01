using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Places;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Instructions
{
    public class LoadNoneInstruction : InstructionWithResult
    {
        public OptionalType Type { get; }

        public LoadNoneInstruction(Place resultPlace, OptionalType type, TextSpan span, Scope scope)
            : base(resultPlace, span, scope)
        {
            Type = type;
        }

        public override string ToInstructionString()
        {
            return $"{ResultPlace} = LOAD[{Type}] none";
        }
    }
}
