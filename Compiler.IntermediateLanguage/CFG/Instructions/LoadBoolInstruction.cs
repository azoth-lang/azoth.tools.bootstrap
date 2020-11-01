using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Places;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG.Instructions
{
    public class LoadBoolInstruction : InstructionWithResult
    {
        public bool Value { get; }

        public LoadBoolInstruction(Place resultPlace, bool value, TextSpan span, Scope scope)
            : base(resultPlace, span, scope)
        {
            Value = value;
        }

        public override string ToInstructionString()
        {
            return $"{ResultPlace} = LOAD[{DataType.Bool}] {Value}";
        }
    }
}
